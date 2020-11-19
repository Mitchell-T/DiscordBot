using Discord.WebSocket;
using DiscordBotV5.Misc.Templates;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBotV5.Services
{
    public class ServerPreferenceService
    {
        private readonly DatabaseService _dbService;

        private readonly Dictionary<Int64, ServerPreference> _preferences = new Dictionary<long, ServerPreference>();

        public ServerPreferenceService(IServiceProvider provider)
        {
            _dbService = provider.GetRequiredService<DatabaseService>();
        }

        public void Initialize()
        {
            Populate();
        }

        // Populate the preferences dictionary when starting the bot
        public void Populate()
        {
            IMongoCollection<BsonDocument> collection = _dbService.GetCollection("ServerSettings");

            var results = collection.Find("{}").ToList();

            foreach(var obj in results)
            {
                ServerPreference preference = BsonSerializer.Deserialize<ServerPreference>(obj);

                _preferences.Add(preference.guild, preference);
            }
        }


        public void ResetCache()
        {
            _preferences.Clear();
            Populate();
        }

        public void UpdatePreferenceCache(Int64 guildID)
        {

        }

        public void CheckPreferenceStatus(SocketGuild guild)
        {
            IMongoCollection<BsonDocument> collection = _dbService.GetCollection("ServerSettings");

            // Search the database for preferences
            var filter = new BsonDocument("guild", Convert.ToInt64(guild.Id));
            var results = collection.Find(filter).ToList();

            // Check if server already has a config
            if (results.Count == 0)
            {
                // Generate new config
                ServerPreference newPreference = new ServerPreference();
                BsonDocument preferenceBson = new BsonDocument();
                preferenceBson = newPreference.ToBsonDocument();
                collection.InsertOne(preferenceBson);
                _preferences.Add(Convert.ToInt64(guild.Id), newPreference);
            }
        }

    }

}
