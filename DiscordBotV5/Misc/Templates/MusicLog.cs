using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Misc.Templates
{
    [BsonIgnoreExtraElements]
    class MusicLog
    {
        // Guild
        public long guild = 0;
        // User
        public long user = 0;
        public string username = "";
        // Song
        public string songTitle = "";
        public string songArtist = "";
        public string songUrl = "";
        public TimeSpan songDuration;
        // Timestamp of the quoted message
        public DateTime date;
    }
}
