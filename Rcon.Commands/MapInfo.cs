namespace Rcon.Commands
{

    public class MapInfo
    {
        /// <summary>
        /// The internal map code (e.g. "de_inferno").
        /// </summary>
        public string MapCode { get; set; }

        /// <summary>
        /// The display name to show in the UI (e.g. "Inferno").
        /// </summary>
        public string DisplayName { get; set; }
    }

    public class MapGroup
    {
        /// <summary>
        /// The name of the map group (e.g. "Competitive Maps").
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The list of maps in this group.
        /// </summary>
        public List<MapInfo> Maps { get; set; }
    }


}
