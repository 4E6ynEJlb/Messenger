using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Button for bot chat. Pressing this will send value of command field to bot
    /// </summary>
    public record BotButton
    {
        public BotButton() { }
        [SetsRequiredMembers]
        public BotButton(BotButtonInfo button)
        {
            Command = button.InnerCommand;
            Text = button.ButtonText;
            BackgroundColor = button.BackgroundColor is not null ? new Color
            {
                R = button.BackgroundColor[0],
                G = button.BackgroundColor[1],
                B = button.BackgroundColor[2]
            } : null;
        }
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
