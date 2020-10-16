using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using DiscordBotV5.Misc;

namespace DiscordBotV5.Modules.Fun
{
    public class QuoteModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private IServiceProvider _service;

        public QuoteModule(IServiceProvider service)
        {
            _service = service;
            _config = service.GetRequiredService<IConfiguration>();
        }

        [Command("addquote")]
        [Alias("aq")]
        public async Task AddQuote(ulong id)
        {
            // Connect to database
            MongoClient dbClient = new MongoClient($"mongodb://{_config["dbUsername"]}:{_config["dbPassword"]}@{_config["dbAddress"]}:{_config["dbPort"]}/?authSource=admin&readPreference=primary");

            IMongoDatabase database = dbClient.GetDatabase("DiscordBot");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Quotes");

            IMessage msg = await Context.Channel.GetMessageAsync(id);

            QuoteTemplate quote = new QuoteTemplate();
            quote.date = msg.Timestamp.DateTime;
            quote.userID = msg.Author.Id.ToString();
            quote.userName = msg.Author.Username;
            quote.guild = Context.Guild.Id.ToString();
            quote.quote = msg.Content;

            BsonDocument quoteBson = new BsonDocument();
            quoteBson = quote.ToBsonDocument();

            await collection.InsertOneAsync(quoteBson);

            IUserMessage m = await Context.Channel.SendMessageAsync("Quote has been added to the database");
            const int delay = 3000;
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        

        [Command("getQuote")]
        [Alias("q")]
        public async Task GetQuote()
        {
            await Context.Message.DeleteAsync();

            // Connect to database and get collection
            MongoClient dbClient = new MongoClient($"mongodb://{_config["dbUsername"]}:{_config["dbPassword"]}@{_config["dbAddress"]}:{_config["dbPort"]}/?authSource=admin&readPreference=primary");
            IMongoDatabase database = dbClient.GetDatabase("DiscordBot");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Quotes");

            var filter = new BsonDocument("guild", Context.Guild.Id.ToString());

            var results = collection.Find(filter).ToList();

            if (results.Count == 0)
            {
                await Context.Channel.SendMessageAsync($"No quotes found for {Context.Guild.Name} \n" +
                    $"please add some quotes for this user first with `$addquote <messageid>`");
                return;
            }

            Random rnd = new Random();
            BsonDocument randomQuote = results[rnd.Next(results.Count - 1)];

            //string userName = randomQuote.GetValue("userName").ToString();
            //long quotedGuild = randomQuote.GetValue("guild").ToInt64();
            string quotedText = randomQuote.GetValue("quote").ToString();
            long quoteduser = randomQuote.GetValue("userID").ToInt64();
            DateTime date = (DateTime)randomQuote.GetValue("date");

            IGuildUser messageAuthor = Context.Guild.GetUser((ulong)quoteduser);

            var embed = new EmbedBuilder();
            embed.WithAuthor(messageAuthor);
            embed.WithColor(new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255)));
            embed.WithTimestamp(date);
            embed.WithDescription($"{quotedText} - **{date.Year}**");

            await Context.Channel.SendMessageAsync("", false, embed.Build());


        }

        [Command("getQuote")]
        [Alias("q")]
        public async Task GetQuote(IGuildUser user)
        {
            await Context.Message.DeleteAsync();

            // Connect to database and get collection
            MongoClient dbClient = new MongoClient($"mongodb://{_config["dbUsername"]}:{_config["dbPassword"]}@{_config["dbAddress"]}:{_config["dbPort"]}/?authSource=admin&readPreference=primary");
            IMongoDatabase database = dbClient.GetDatabase("DiscordBot");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Quotes");

            var filter = new BsonDocument("guild", Context.Guild.Id.ToString());

            var results = collection.Find(filter).ToList();
            Console.WriteLine(results.Count);

            List<BsonDocument> actualResults = new List<BsonDocument>();

            foreach (var result in results)
            {
                if (result.GetValue("userID").ToInt64() == (long)user.Id)
                {
                    actualResults.Add(result);
                }
            }
            if(actualResults.Count == 0)
            {
                await Context.Channel.SendMessageAsync($"No quotes found for {user.Mention} \n" +
                    $"please add some quotes for this user first with `$addquote <messageid>`");
                return;
            }

            Random rnd = new Random();
            BsonDocument randomQuote = actualResults[rnd.Next(actualResults.Count - 1)];

            string quotedText = randomQuote.GetValue("quote").ToString();
            long quoteduser = randomQuote.GetValue("userID").ToInt64();
            DateTime date = (DateTime)randomQuote.GetValue("date");

            IGuildUser messageAuthor = Context.Guild.GetUser((ulong)quoteduser);

            var embed = new EmbedBuilder();
            embed.WithAuthor(messageAuthor);
            embed.WithColor(new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255)));
            embed.WithTimestamp(date);
            embed.WithDescription($"{quotedText} - **{date.Year}**");

            await Context.Channel.SendMessageAsync("", false, embed.Build());


        }

    }

}
