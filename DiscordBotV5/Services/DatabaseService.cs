using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Services
{
    class DatabaseService
    {
        private readonly IConfiguration _config;
        private MongoClient _dbClient;
        public IMongoDatabase database;


        public DatabaseService(IServiceProvider service)
        {
            _config = service.GetRequiredService<IConfiguration>();
        }

        public void Initialize()
        {
            // Connect to database
            _dbClient = new MongoClient($"mongodb://{_config["dbUsername"]}:{_config["dbPassword"]}@{_config["dbAddress"]}:{_config["dbPort"]}/?authSource=admin&readPreference=primary");
            database = _dbClient.GetDatabase("DiscordBot");
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return database.GetCollection<BsonDocument>(collectionName);
        }

    }
}
