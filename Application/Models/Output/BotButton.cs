namespace Application.Models.Output
{
    /// <summary>
    /// Button for bot chat. Pressing this will send value of command field to bot
    /// </summary>
    public record BotButton
    {
        /// <summary>
        /// Visible for user text on button. Length should be between 1 and 16 chars
        /// </summary>
        public required string Text { get; init; }
        /// <summary>
        /// Sending to bot command when user presses the button. Length should be between 1 and 16 chars
        /// </summary>
        public required string Command { get; init; }
        /// <summary>
        /// Color of button. If null, default color will be used. 
        /// </summary>
        public required Color? BackgroundColor { get; init; }
    }
}
