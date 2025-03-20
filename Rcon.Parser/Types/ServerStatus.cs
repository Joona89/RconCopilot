namespace Rcon.Parser.Types
{
    /// <summary>
    /// Represents the parsed server status information.
    /// </summary>
    public class ServerStatusDto
    {
        /// <summary>
        /// Basic server fields from the status command.
        /// </summary>
        public string Server { get; set; }           // e.g. "Running [127.0.0.1:27015]"
        public string Client { get; set; }           // e.g. "Disconnected"
        public string Current { get; set; }          // e.g. "game"
        public string Source { get; set; }           // e.g. "console"
        public string Hostname { get; set; }         // e.g. "Server name"
        public string Spawn { get; set; }            // e.g. "5" or "6"
        public string Version { get; set; }          // e.g. "1.40.7.1/14071 10329 secure  public"
        public string SteamId { get; set; }          // e.g. ""
        public string UdpIp { get; set; }            // e.g. "127.0.0.1:27015"
        public string OsType { get; set; }           // e.g. "Linux dedicated"
        public string PlayersSummary { get; set; }   // e.g. "1 humans, 1 bots (0 max) (not hibernating) (unreserved)"

        /// <summary>
        /// Any additional fields that might be parsed from the status.
        /// </summary>
        public Dictionary<string, string> AdditionalInfo { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Parsed players from the status.
        /// </summary>
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();

        /// <summary>
        /// The time this status data was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Represents a single player as parsed from the server status.
    /// </summary>
    public class PlayerDto
    {
        /// <summary>
        /// The id for the player (or bot).  
        /// Note: Although often numeric, this is a string to accommodate special cases.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The connection time (or time in game) as reported in status.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// The reported ping.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// The reported loss.
        /// </summary>
        public int Loss { get; set; }

        /// <summary>
        /// The state (e.g., "active", "challenging", "BOT").
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The network rate.
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// The address of the client.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The display name of the player.
        /// </summary>
        public string Name { get; set; }
    }
}

