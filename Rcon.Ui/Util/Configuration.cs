using Rcon.Parser.Types;
using Rcon.Ui.Types;

namespace Rcon.Util
{
    public static class Configuration
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
                    new MapInfo { MapCode = "de_anubis", DisplayName = "Anubis" },
                    new MapInfo { MapCode = "de_ancient", DisplayName = "Ancient" },
                    new MapInfo { MapCode = "de_mirage", DisplayName = "Mirage" },
                    new MapInfo { MapCode = "de_nuke", DisplayName = "Nuke" },
                    new MapInfo { MapCode = "de_train", DisplayName = "Train" },
                    new MapInfo { MapCode = "de_overpass", DisplayName = "Overpass" },
                    new MapInfo { MapCode = "de_vertigo", DisplayName = "Vertigo" },
                    new MapInfo { MapCode = "de_overpass", DisplayName = "Overpass" },
                    new MapInfo { MapCode = "de_edin", DisplayName = "Edin" },
                    new MapInfo { MapCode = "cs_office", DisplayName = "Office" },
                    new MapInfo { MapCode = "cs_italy", DisplayName = "Italy" },
                }
            },
            new MapGroup
            {
                GroupName = "Wingman Maps",
                Maps = new List<MapInfo>
                {
                    new MapInfo { MapCode = "de_whistle", DisplayName = "Whistle" },
                    new MapInfo { MapCode = "de_palais", DisplayName = "Palais" },

                }
            }
        };
        // List of predefined commands with category assignments.
        public static readonly PredefinedCommand[] Values =
        {

            new PredefinedCommand { DisplayText = "Change CT Name",     CommandValue = "mp_teamname_1", Category = "Team", RequiresUserInputParameter = true },
            new PredefinedCommand { DisplayText = "Change T Name",     CommandValue = "mp_teamname_2", Category = "Team", RequiresUserInputParameter = true },
            new PredefinedCommand { DisplayText = "Restart Game",   CommandValue = "mp_restartgame 1", Category = "Game" },
            new PredefinedCommand { DisplayText = "Warmup End",     CommandValue = "mp_warmup_end", Category = "Game" },
            new PredefinedCommand { DisplayText = "Game Pause",     CommandValue = "mp_pause_match", Category = "Game" },
            new PredefinedCommand { DisplayText = "Game Unpause",     CommandValue = "mp_unpause_match", Category = "Game" },
            new PredefinedCommand { DisplayText = "Scramble teams",     CommandValue = "mp_scrambleteams", Category = "Game" },

            // Use the generic change level command.
            new PredefinedCommand { DisplayText = "Game mode",     CommandValue = "game_mode", Category = "Game mode", PossibleParameters = new [] { "0:Casual", "1:Competitive", "2:Scrim" } },
            new PredefinedCommand { DisplayText = "Game type",     CommandValue = "game_type", Category = "Game mode", PossibleParameters = new [] { "0:Bomb defusal", "1:Fun", "2:Training",  "3:Custom", "4:Co-op", "5:Skirmish" } },
            new PredefinedCommand { DisplayText = "Change Map",     CommandValue = "changelevel", Category = "Map" },
            new PredefinedCommand
            {
                DisplayText = "Workshop Map",
                CommandValue = "host_workshop_map",
                PossibleParameters = new [] { "3437809122:New Cache" },
                Category = "Map"
            },
            new PredefinedCommand { DisplayText = "Round Time",     CommandValue = "mp_roundtime", PossibleParameters = new [] { "3:3 Seconds", "5:5 Seconds", "7:7 Seconds", "10:10 Seconds" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Buy Time",       CommandValue = "mp_buytime", PossibleParameters = new [] { "15:15 Seconds", "20:20 Seconds", "30:30 Seconds" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Buy Anywhere",   CommandValue = "mp_buy_anywhere", PossibleParameters = new [] { "1:Yes", "0:No" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Team Balance",   CommandValue = "mp_autoteambalance", PossibleParameters = new [] { "1:Yes", "0:No" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Limit Teams",    CommandValue = "mp_limitteams", PossibleParameters = new [] { "0:No", "1:Yes" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Friendly Fire",  CommandValue = "mp_friendlyfire", PossibleParameters = new [] { "1:Yes", "0:No" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Force AutoBalance", CommandValue = "mp_forceautoteambalance", PossibleParameters = new [] { "1:Yes", "0:No" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Freeze Time",    CommandValue = "mp_freezetime", PossibleParameters = new [] { "0:0 Seconds", "5:5 Seconds", "10:10 Seconds", "15:15 Seconds" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Time Limit",     CommandValue = "mp_timelimit", PossibleParameters = new [] { "30:30 Seconds", "45:45 Seconds", "60:60 Seconds" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Max Rounds",     CommandValue = "mp_maxrounds", PossibleParameters = new [] { "12", "16", "20" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Round Restart Delay", CommandValue = "mp_roundrestartdelay", PossibleParameters = new [] { "3:3 Seconds", "5:5 Seconds", "7:7 Seconds" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "All Talk",       CommandValue = "sv_alltalk", PossibleParameters = new [] { "1:Enabled", "0:Disabled" }, Category = "Config" },
            new PredefinedCommand { DisplayText = "Kick Bots",      CommandValue = "bot_kick", Category = "Bots" },
            new PredefinedCommand { DisplayText = "Add Bots",      CommandValue = "bot_add", Category = "Bots" },
            new PredefinedCommand { DisplayText = "Stop Bots",      CommandValue = "bot_stop", PossibleParameters = new [] { "1", "0" }, Category = "Bots" },
            new PredefinedCommand { DisplayText = "Cheats",         CommandValue = "sv_cheats", PossibleParameters = new [] { "1:Enabled", "0:Disabled" }, Category = "Cheats" },
            new PredefinedCommand { DisplayText = "Gravity",        CommandValue = "sv_gravity", PossibleParameters = new [] { "400", "600", "800", "1000" }, Category = "Cheats" },
            new PredefinedCommand { DisplayText = "Max Speed",      CommandValue = "sv_maxspeed", PossibleParameters = new [] { "200", "250", "300", "350", "500", "800", "1000" }, Category = "Cheats" },
            new PredefinedCommand { DisplayText = "Enable BunnyHopping",      CommandValue = "sv_enablebunnyhopping", PossibleParameters = new [] { "1:Enabled", "0:Disabled" }, Category = "Cheats" },
            new PredefinedCommand { DisplayText = "Auto BunnyHopping",      CommandValue = "sv_autobunnyhopping ", PossibleParameters = new [] { "1:Enabled", "0:Disabled" }, Category = "Cheats" },
            
        };
    }
}
