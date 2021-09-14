using System;

namespace DiscordBotV5.Services.Attributes
{
    public sealed class HelpDisplay : Attribute
    {
        public HelpDisplayOptions Option { get; }

        public HelpDisplay(HelpDisplayOptions option)
        {
            Option = option;
        }
    }
}