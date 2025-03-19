using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


namespace Rcon.Server
{
    public static class ServerResponseParser
    {
        public static ServerStatusDto ParseStatusResponseDto(string response)
        {
            var dto = new ServerStatusDto();

            // Marker separating server info from players.
            string marker = "---------players--------";
            int markerIndex = response.IndexOf(marker);
            string serverInfoPart;
            string playersPart = "";

            if (markerIndex >= 0)
            {
                serverInfoPart = response.Substring(0, markerIndex);
                playersPart = response.Substring(markerIndex + marker.Length);
            }
            else
            {
                serverInfoPart = response;
            }

            // Parse server info
            var serverLines = serverInfoPart.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in serverLines)
            {
                if (!line.Contains(":"))
                    continue;
                var parts = line.Split(new char[] { ':' }, 2);
                if (parts.Length != 2)
                    continue;
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                // Skip keys we don't need.
                string lowerKey = key.ToLower();
                if (lowerKey.Contains("spawngroup") || lowerKey.Contains("prefab") || key.StartsWith("-----"))
                    continue;

                // Map known keys to properties.
                if (key.StartsWith("Server", StringComparison.OrdinalIgnoreCase))
                    dto.Server = value;
                else if (key.StartsWith("Client", StringComparison.OrdinalIgnoreCase))
                    dto.Client = value;
                else if (key.StartsWith("@ Current", StringComparison.OrdinalIgnoreCase) || key.StartsWith("Current", StringComparison.OrdinalIgnoreCase))
                    dto.Current = value;
                else if (key.StartsWith("source", StringComparison.OrdinalIgnoreCase))
                    dto.Source = value;
                else if (key.StartsWith("hostname", StringComparison.OrdinalIgnoreCase))
                    dto.Hostname = value;
                else if (key.StartsWith("spawn", StringComparison.OrdinalIgnoreCase))
                    dto.Spawn = value;
                else if (key.StartsWith("version", StringComparison.OrdinalIgnoreCase))
                    dto.Version = value;
                else if (key.StartsWith("steamid", StringComparison.OrdinalIgnoreCase))
                    dto.SteamId = value;
                else if (key.StartsWith("udp/ip", StringComparison.OrdinalIgnoreCase))
                    dto.UdpIp = value;
                else if (key.StartsWith("os/type", StringComparison.OrdinalIgnoreCase))
                    dto.OsType = value;
                else if (key.StartsWith("players", StringComparison.OrdinalIgnoreCase))
                    dto.PlayersSummary = value;
                else
                    dto.AdditionalInfo[key] = value;
            }
            dto.LastUpdated = DateTime.Now;

            // Parse players.  
            // Example player section line (with header removed) might look like:
            // "0    02:14    0    0     active 786432 192.168.1.40:62222 'Himppu'"
            if (!string.IsNullOrWhiteSpace(playersPart))
            {
                var playerLines = playersPart.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in playerLines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed))
                        continue;
                    if (trimmed.StartsWith("id", StringComparison.OrdinalIgnoreCase) ||
                        trimmed.StartsWith("#end", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var tokens = trimmed.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length < 2)
                        continue;

                    PlayerDto player = new PlayerDto();

                    // We'll try to parse detailed fields if available.  
                    if (tokens.Length >= 8)
                    {
                        player.Id = tokens[0].Trim();
                        player.Time = tokens[1].Trim();
                        if (int.TryParse(tokens[2], out int ping))
                            player.Ping = ping;
                        if (int.TryParse(tokens[3], out int loss))
                            player.Loss = loss;
                        player.State = tokens[4].Trim();
                        if (int.TryParse(tokens[5], out int rate))
                            player.Rate = rate;
                        player.Address = tokens[6].Trim();
                        // Name is assumed to be the last token, with surrounding quotes removed.
                        player.Name = tokens[tokens.Length - 1].Trim('\'', '"');
                    }
                    else
                    {
                        // Fallback if we don't have many tokens.
                        player.Id = tokens[0].Trim();
                        player.Name = tokens[tokens.Length - 1].Trim('\'', '"');
                    }
                    if(player.Id != "65535")
                    {
                        dto.Players.Add(player);
                    }
                    
                }
            }
            return dto;
        }


    }
}
