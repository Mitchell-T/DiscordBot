using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
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
        public long quoteChannel = 0;
        // Discord status checker
        public bool statusCheckEnabled = false;
        public int statusCheckLevel = 0;
        // Music
        public bool musicEnabled = true;
        public bool onlyInMusicChannel = false;
        public long musicChannel = 0;
        // Prefix
        public string prefix = "";
        // Welcome messages
        public bool welcomeEnabled = false;
        public long welcomeChannel = 0;
        // Chat filter
        public bool chatFilterEnabled = false;

    }
}
