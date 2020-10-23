using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotV5.Misc.Templates
{
    class ServerPreference
    {
        // Guild id
        public long guild = 0;
        // NSFW toggle
        public bool nsfwEnabled = true;
        // Suggestions
        public bool suggestionsEnabled = false;
        public long suggestionChannel = 0;
        // Quotes
        public bool quotesEnabled = false;
        // Discord status checker
        public bool statusCheckEnabled = false;
        public int statusCheckLevel = 0;
        // Music
        public bool musicEnabled = true;
        public bool onlyInMusicChannel = false;
        public long musicChannel = 0;

    }
}
