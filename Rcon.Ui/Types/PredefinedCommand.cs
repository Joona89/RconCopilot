namespace Rcon.Ui.Types
{
    public class PredefinedCommand
    {
        public required string DisplayText { get; set; }          // Localized display text
        public required string CommandValue { get; set; }         // Actual command to execute
        public string[] PossibleParameters { get; set; } // Array of predefined parameter values, if any
        /// <summary>
        /// When true, prompt the user for free-form parameter input.
        /// </summary>
        public bool RequiresUserInputParameter { get; set; } = false;
        public required string Category { get; set; }             // Category name (e.g. "General", "Game", etc.)
    }

}
