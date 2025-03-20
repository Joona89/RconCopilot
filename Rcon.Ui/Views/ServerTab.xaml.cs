using System.Collections.ObjectModel;
using Rcon.Client;
using Rcon.Parser.Types;
using Rcon.Server;
using Rcon.Ui.Types;
using Rcon.Util;

namespace Rcon.Ui.Views
{
    public partial class ServerTab : ContentPage
    {
        private RconClient _rconClient;
        private ObservableCollection<CommandHistoryItem> _commandHistory;
        private CancellationTokenSource _statusCts;

        // Observable collection for players (bound to the CollectionView on the right).
        public ObservableCollection<PlayerDto> Players { get; set; } = new ObservableCollection<PlayerDto>();

        // We'll store the commands grouped by category.
        private Dictionary<string, List<PredefinedCommand>> _categories;


        public ServerTab()
        {
            InitializeComponent();
            _commandHistory = new ObservableCollection<CommandHistoryItem>();
            // Bind the player collection to the CollectionView.
            PlayerListView.ItemsSource = Players;
            PopulatePredefinedCommands();
        }

        // Group the predefined commands by category and create one Picker (dropdown) per category.
        private void PopulatePredefinedCommands()
        {
            _categories = Configuration.Values
                .GroupBy(cmd => cmd.Category)
                .ToDictionary(g => g.Key, g => g.ToList());

            PredefinedCommandsStack.Children.Clear();

            foreach (var category in _categories.Keys)
            {
                // Create a Picker for each category.
                var picker = new Picker
                {
                    Title = category,
                    HorizontalOptions = LayoutOptions.Start,
                    WidthRequest = 140
                };

                // Bind the ItemsSource to the list of commands for this category.
                picker.ItemsSource = _categories[category];
                picker.ItemDisplayBinding = new Binding("DisplayText");

                // When a command is selected from the dropdown, process it.
                picker.SelectedIndexChanged += async (s, e) =>
                {
                    var p = s as Picker;
                    if (p.SelectedIndex == -1)
                        return;
                    var cmd = p.SelectedItem as PredefinedCommand;
                    await ProcessCommand(cmd);
                    p.SelectedIndex = -1; // reset selection
                };

                PredefinedCommandsStack.Children.Add(picker);
            }
        }

        private async Task ProcessCommand(PredefinedCommand cmd)
        {
            string fullCommand = cmd.CommandValue;

            if (cmd.CommandValue == "changelevel")
            {
                // Existing changelevel logic (e.g., select map group --> map)
                string[] groupOptions = Configuration.MapGroups.Select(g => g.GroupName).ToArray();
                string selectedGroup = await DisplayActionSheet("Select Map Group", "Cancel", null, groupOptions);
                if (string.IsNullOrWhiteSpace(selectedGroup) || selectedGroup == "Cancel")
                    return;

                var mapGroup = Configuration.MapGroups.FirstOrDefault(g => g.GroupName == selectedGroup);
                if (mapGroup == null || mapGroup.Maps.Count == 0)
                    return;

                string[] mapOptions = mapGroup.Maps.Select(m => m.DisplayName).ToArray();
                string selectedMap = await DisplayActionSheet("Select Map", "Cancel", null, mapOptions);
                if (string.IsNullOrWhiteSpace(selectedMap) || selectedMap == "Cancel")
                    return;

                var map = mapGroup.Maps.FirstOrDefault(m => m.DisplayName == selectedMap);
                if (map == null)
                    return;

                fullCommand += " " + map.MapCode;
            }
            else if (cmd.CommandValue == "host_workshop_map" || (cmd.PossibleParameters != null && cmd.PossibleParameters.Any(y => y.Contains(":"))))
            {
                // Build a list of workshop options.
                List<string> workshopOptions = new List<string>();
                if (cmd.PossibleParameters != null && cmd.PossibleParameters.Length > 0)
                {
                    // For each parameter, if it contains a colon, extract the pretty name.
                    foreach (var param in cmd.PossibleParameters)
                    {
                        var parts = param.Split(':');
                        string displayOption = parts.Length == 2 ? parts[1] : param;
                        workshopOptions.Add(displayOption);
                    }
                }
                // Add a custom option to allow a user-entered ID.
                workshopOptions.Add("Custom");

                // Prompt the user to select between the predefined option(s) and custom.
                string selected = await DisplayActionSheet("Select Workshop Map", "Cancel", null, workshopOptions.ToArray());
                if (string.IsNullOrWhiteSpace(selected) || selected == "Cancel")
                    return;

                if (selected == "Custom")
                {
                    // Prompt for a custom workshop ID.
                    string customId = await DisplayPromptAsync("Custom Workshop Map", "Enter workshop map ID:");
                    if (string.IsNullOrWhiteSpace(customId))
                        return;
                    fullCommand += " " + customId;
                }
                else
                {
                    // Find the matching predefined parameter.
                    string matchingParam = cmd.PossibleParameters.FirstOrDefault(p =>
                    {
                        var parts = p.Split(':');
                        return (parts.Length == 2 && parts[1] == selected) || p == selected;
                    });
                    if (matchingParam != null)
                    {
                        string mapId = matchingParam.Contains(":") ? matchingParam.Split(':')[0] : matchingParam;
                        fullCommand += " " + mapId;
                    }
                }
            }
            else if (cmd.PossibleParameters != null && cmd.PossibleParameters.Length > 0)
            {
                string selectedParam = await DisplayActionSheet($"Select parameter for {cmd.DisplayText}", "Cancel", null, cmd.PossibleParameters);
                if (string.IsNullOrWhiteSpace(selectedParam) || selectedParam == "Cancel")
                    return;
                fullCommand += " " + selectedParam;
            }
            else if (cmd.RequiresUserInputParameter)
            {
                string inputParam = await DisplayPromptAsync("Parameter Required", $"Enter parameter for {cmd.DisplayText}:");
                if (string.IsNullOrWhiteSpace(inputParam))
                    return;
                fullCommand += " " + inputParam;
            }

            await ExecuteCommandAsync(fullCommand);
        }



        // Inside ConnectButton_Clicked:
        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            var ip = EntryIP.Text;
            if (!int.TryParse(EntryPort.Text, out int port))
            {
                await DisplayAlert("Error", "Invalid port number.", "OK");
                return;
            }
            var password = EntryPassword.Text;
            _rconClient = new RconClient(ip, port, password);
            try
            {
                await _rconClient.ConnectAsync();
                ConnectionForm.IsVisible = false;
                ConnectedArea.IsVisible = true;
                // Update the tab title to show the IP and an active (green) indicator.
                this.Title = "🟢 " + ip;
                // Start background status updates.
                _statusCts = new CancellationTokenSource();
                StartBackgroundStatusLoop(_statusCts.Token);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection Error", ex.Message, "OK");
            }
        }

        private void UpdateStatusUI(ServerStatusDto status)
        {
            // Clear current server info UI.
            ServerInfoStack.Children.Clear();
            // Update the Last Updated label.
            LastUpdatedLabel.Text = "Last updated: " + status.LastUpdated.ToString("HH:mm:ss");

            // Display known server fields.
            void AddInfo(string key, string value)
            {
                var fs = new FormattedString();
                fs.Spans.Add(new Span { Text = key + ": ", FontAttributes = FontAttributes.Bold, FontSize = 14 });
                fs.Spans.Add(new Span { Text = value, FontSize = 14 });
                var lbl = new Label { FormattedText = fs, Margin = new Thickness(0, 1) };
                ServerInfoStack.Children.Add(lbl);
            }

            if (!string.IsNullOrEmpty(status.Server))
                AddInfo("Server", status.Server);
            if (!string.IsNullOrEmpty(status.Client))
                AddInfo("Client", status.Client);
            if (!string.IsNullOrEmpty(status.Current))
                AddInfo("Current", status.Current);
            if (!string.IsNullOrEmpty(status.Source))
                AddInfo("Source", status.Source);
            if (!string.IsNullOrEmpty(status.Hostname))
                AddInfo("Hostname", status.Hostname);
            if (!string.IsNullOrEmpty(status.Spawn))
                AddInfo("Spawn", status.Spawn);
            if (!string.IsNullOrEmpty(status.Version))
                AddInfo("Version", status.Version);
            if (!string.IsNullOrEmpty(status.SteamId))
                AddInfo("SteamId", status.SteamId);
            if (!string.IsNullOrEmpty(status.UdpIp))
                AddInfo("UDP/IP", status.UdpIp);
            if (!string.IsNullOrEmpty(status.OsType))
                AddInfo("OS/Type", status.OsType);
            if (!string.IsNullOrEmpty(status.PlayersSummary))
                AddInfo("Players", status.PlayersSummary);

            // Show all additional fields.
            foreach (var kvp in status.AdditionalInfo)
            {
                AddInfo(kvp.Key, kvp.Value);
            }

            // Bind the Players collection to the CollectionView.
            UpdatePlayers(status.Players);
        }

        private void UpdatePlayers(List<PlayerDto> newPlayers)
        {
            var newList = newPlayers.ToList();

            // Remove items that are no longer present.
            for (int i = Players.Count - 1; i >= 0; i--)
            {
                var existing = Players[i];
                if (!newList.Any(newItem => newItem.Id == existing.Id && newItem.Name == existing.Name))
                {
                    Players.RemoveAt(i);
                }
            }

            // Add items that are new.
            foreach (var newPlayer in newList)
            {
                if (!Players.Any(existing => existing.Id == newPlayer.Id && existing.Name == newPlayer.Name))
                {
                    Players.Add(newPlayer);
                }
            }

            // Optionally: update items that have changed if you have additional properties.
        }




        // Background status update loop: every 5 seconds, send a "status" command,
        // parse the response to update the player list, but do NOT log it into the output window.
        private async void StartBackgroundStatusLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    string response = await _rconClient.SendCommandAsync("status", token);
                    var statusDto = ServerResponseParser.ParseStatusResponseDto(response);
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateStatusUI(statusDto);
                    });
                }
                catch (Exception ex)
                {
                    // You may choose to handle exceptions (or simply ignore and continue).
                }
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
                catch (TaskCanceledException) { }
            }
        }


        // Handler for the free-text Send button (only user-entered commands get logged).
        private async void SendCommandButton_Clicked(object sender, EventArgs e)
        {
            var command = CommandEntry.Text;
            if (string.IsNullOrWhiteSpace(command))
                return;
            CommandEntry.Text = string.Empty;
            await ExecuteCommandAsync(command);
        }

        // Executes a command that the user has sent, logging its response in the output window.
        private async Task ExecuteCommandAsync(string command)
        {
            try
            {
                string response = await _rconClient.SendCommandAsync(command, CancellationToken.None);
                AddCommandToHistory(command, response);
            }
            catch (OperationCanceledException)
            {
                AddCommandToHistory(command, "Command canceled.");
            }
            catch (Exception ex)
            {
                AddCommandToHistory(command, "Error: " + ex.Message);
            }
        }

        // Append a user-sent command and its final response to the output window.
        private void AddCommandToHistory(string command, string response)
        {
            var historyItem = new CommandHistoryItem
            {
                Command = command,
                Response = response,
                Timestamp = DateTime.Now
            };

            var label = new Label
            {
                Text = $"{historyItem.Timestamp:T} Command: {command}\nResponse: {response}",
                FontSize = 12
            };

            var frame = new Frame
            {
                BorderColor = Colors.Gray,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 5),
                Content = label
            };

            OutputStack.Children.Add(frame);
            _commandHistory.Add(historyItem);
        }

        private async void KickButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is PlayerDto player)
            {
                bool confirm = await DisplayAlert("Kick Player", $"Kick {player.Name}?", "Yes", "No");
                if (confirm)
                {
                    // Use the player's name instead of player.Id
                    await ExecuteCommandAsync("kick " + player.Name);
                }
            }
        }

        private async void BanButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is PlayerDto player)
            {
                bool confirm = await DisplayAlert("Ban Player", $"Ban {player.Name}?", "Yes", "No");
                if (confirm)
                {
                    // Optionally prompt for extra ban parameters (e.g., duration)
                    string banParam = await DisplayPromptAsync("Ban Parameter", $"Enter ban parameter for {player.Name} (e.g., duration):");
                    if (!string.IsNullOrWhiteSpace(banParam))
                    {
                        // Use the player's name in the ban command
                        await ExecuteCommandAsync("banid " + banParam + " " + player.Name);
                    }
                }
            }
        }

        // Cancel the background status loop when the page is disappearing.
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _statusCts?.Cancel();
        }
    }

    public class CommandHistoryItem
    {
        public string Command { get; set; }
        public string Response { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
