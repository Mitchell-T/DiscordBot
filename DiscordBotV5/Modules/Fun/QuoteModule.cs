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

namespace DiscordBotV5.Modules.Fun
{
    public class QuoteModule : ModuleBase<SocketCommandContext>
    {
        string dbUsername, dbPassword, dbAddress, dbPort;

        [Command("addquote")]
        [Alias("aq")]
        public async Task AddQuote(ulong id)
        {
            // Connect to database
            MongoClient dbClient = new MongoClient($"mongodb://{dbUsername}:{dbPassword}@{dbAddress}:{dbPort}/?authSource=myUserAdmin");

            //List<Quote> quoteList = 

            // Retrieve message from id
            IMessage message = await Context.Channel.GetMessageAsync(id);
            // Turn message into object
            Quote newQuote = new Quote(message.Content, message.Timestamp.DateTime.ToShortDateString(), message.Author.Id.ToString());
            // 

            // Add to JSON
            string json = JsonConvert.SerializeObject(newQuote, Formatting.Indented);
            Console.WriteLine(json);
        }

    }

    public class Quote
    {
        public string QuotedText { get; set; }
        public string Date { get; set; }
        public string User { get; set; }
        public Quote(string quotedText, string date, string user)
        {
            QuotedText = quotedText;
            Date = date;
            User = user;
        }
    }
}
