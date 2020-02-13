using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordBotV5.Modules.Fun
{
    public class QuoteModule : ModuleBase<SocketCommandContext>
    {

        [Command("addquote")]
        [Alias("aq")]
        public async Task AddQuote(ulong id)
        {
            // Check for file and open
            if (!File.Exists("Storage/Quotes.json"))
            {
                File.Create("Storage/Quotes.json");
            }
            FileStream file = File.Open("Storage/Quotes.json", FileMode.Open);
            string jsonfile = File.ReadAllText("Storage/Quotes.json");

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
