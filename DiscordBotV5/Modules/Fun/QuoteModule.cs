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
            List<Quote> quoteList = new List<Quote>();

            // Retrieve message from id
            IMessage message = await Context.Channel.GetMessageAsync(id);
            // Turn message into object
            Quote newQuote = new Quote(message.Content, message.Timestamp, message.Author);
            // 



            // add to json
        }

    }

    public class Quote
    {
        public string QuotedText { get; set; }
        public DateTimeOffset Date { get; set; }
        public IUser User { get; set; }
        public Quote(string quotedText, DateTimeOffset date, IUser user)
        {
            QuotedText = quotedText;
            Date = date;
            User = user;
        }
    }
}
