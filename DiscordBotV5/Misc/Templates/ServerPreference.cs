using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotV5.Misc.Templates
{
    class ServerPreference
    {
        // Guild id
        public string guild;
        // NSFW toggle
        public bool nsfwEnabled = true;
        // Suggestions
        public bool suggestionsEnabled = false;
        public string suggestionChannel;
        // Quotes
        public bool quotesEnabled = false;
        // Discord status checker
        public bool statusCheckEnabled = false;
        public int statusCheckLevel = 0;
    }
}
