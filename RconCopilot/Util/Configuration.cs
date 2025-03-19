using Rcon.Commands;

namespace Rcon.Util
{
    public class Configuration
    {

        public static readonly List<MapGroup> MapGroups = new List<MapGroup>
        {
            new MapGroup
            {
                GroupName = "Competitive Maps",
                Maps = new List<MapInfo>
                {
                    new MapInfo { MapCode = "de_dust2", DisplayName = "Dust II" },
                    new MapInfo { MapCode = "de_inferno", DisplayName = "Inferno" },
                    new MapInfo { MapCode = "de_mirage", DisplayName = "Mirage" },
                    new MapInfo { MapCode = "de_nuke", DisplayName = "Nuke" },
                    new MapInfo { MapCode = "de_train", DisplayName = "Train" },
                    new MapInfo { MapCode = "de_overpass", DisplayName = "Overpass" },
                    new MapInfo { MapCode = "de_vertigo", DisplayName = "Vertigo" }
                }
            },
            new MapGroup
            {
                GroupName = "Wingman Maps",
                Maps = new List<MapInfo>
                {
                    new MapInfo { MapCode = "de_shortdust", DisplayName = "Short Dust" },
                    new MapInfo { MapCode = "de_shorttrain", DisplayName = "Short Train" },
                    new MapInfo { MapCode = "de_shortnuke", DisplayName = "Short Nuke" }
                }
            }
        };
        // List of predefined commands with category assignments.
        public static readonly PredefinedCommand[] Values =
        {
            new PredefinedCommand { DisplayText = "Status",         CommandValue = "status", Category = "General" },
            new PredefinedCommand { DisplayText = "Kick",           CommandValue = "kick",   RequiresUserInputParameter = true, Category = "Player" },
            new PredefinedCommand { DisplayText = "Ban",            CommandValue = "banid",  RequiresUserInputParameter = true, Category = "Player" },
            new PredefinedCommand { DisplayText = "Restart Game",   CommandValue = "mp_restartgame", PossibleParameters = new [] { "1" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Warmup End",     CommandValue = "mp_warmup_end", Category = "Game" },
            // Use the generic change level command.
            new PredefinedCommand { DisplayText = "Change Map",     CommandValue = "changelevel", Category = "Game" },
            new PredefinedCommand
            {
                DisplayText = "Workshop Map",
                CommandValue = "host_workshop_map",
                PossibleParameters = new [] { "3437809122:New Cache" },
                Category = "Game"
            },
            new PredefinedCommand { DisplayText = "Round Time",     CommandValue = "mp_roundtime", PossibleParameters = new [] { "3", "5", "7", "10" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Buy Time",       CommandValue = "mp_buytime", PossibleParameters = new [] { "15", "20", "30" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Buy Anywhere",   CommandValue = "mp_buy_anywhere", PossibleParameters = new [] { "1", "0" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Team Balance",   CommandValue = "mp_autoteambalance", PossibleParameters = new [] { "1", "0" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Limit Teams",    CommandValue = "mp_limitteams", PossibleParameters = new [] { "0", "1" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Friendly Fire",  CommandValue = "mp_friendlyfire", PossibleParameters = new [] { "1", "0" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Force AutoBalance", CommandValue = "mp_forceautoteambalance", PossibleParameters = new [] { "1", "0" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Freeze Time",    CommandValue = "mp_freezetime", PossibleParameters = new [] { "0", "5", "10", "15" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Time Limit",     CommandValue = "mp_timelimit", PossibleParameters = new [] { "30", "45", "60" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Max Rounds",     CommandValue = "mp_maxrounds", PossibleParameters = new [] { "12", "16", "20" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Round Restart Delay", CommandValue = "mp_roundrestartdelay", PossibleParameters = new [] { "3", "5", "7" }, Category = "Game" },
            new PredefinedCommand { DisplayText = "Kick Bots",      CommandValue = "bot_kick", Category = "Bots" },
            new PredefinedCommand { DisplayText = "Stop Bots",      CommandValue = "bot_stop", PossibleParameters = new [] { "1", "0" }, Category = "Bots" },
            new PredefinedCommand { DisplayText = "Cheats",         CommandValue = "sv_cheats", PossibleParameters = new [] { "1", "0" }, Category = "Server" },
            new PredefinedCommand { DisplayText = "Gravity",        CommandValue = "sv_gravity", PossibleParameters = new [] { "400", "600", "800", "1000" }, Category = "Server" },
            new PredefinedCommand { DisplayText = "Max Speed",      CommandValue = "sv_maxspeed", PossibleParameters = new [] { "200", "250", "300", "350" }, Category = "Server" },
            new PredefinedCommand { DisplayText = "All Talk",       CommandValue = "sv_alltalk", PossibleParameters = new [] { "1", "0" }, Category = "Server" }
        };
    }
}
