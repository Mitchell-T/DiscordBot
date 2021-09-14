using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DiscordBotV5.Modules.Fun
{
    [Name("Magic 8 Ball")]
    [Summary("Various magic 8-Ball commands")]
    public class MagicEightBallModule : ModuleBase<SocketCommandContext>
    {
        [Command("8ball")]
        [Alias("eightball", "8b", "8-ball", "8-b")]
        [Summary("Ask 8Ball a question")]
        public async Task EightBall([Remainder] string question = "")
        {
            var embed = new EmbedBuilder();

            if (!question.Contains("?"))
                question += "?";

            await Context.Message.DeleteAsync();

            string APILocation = $"https://8ball.delegator.com/magic/XML/{question}";

            XmlDocument eightBallXML = new XmlDocument();
            eightBallXML.Load(APILocation);
            XmlNode root = eightBallXML;
            XmlNode node = root.ChildNodes[1];
            Console.WriteLine(node.ChildNodes[1].InnerText);
            
            string emoji = "";
            switch (node.ChildNodes[2].InnerText)
            {
                case "Neutral":
                    emoji = ":heavy_minus_sign:";
                    embed.Color = Color.LighterGrey;
                    break;
                case "Affirmative":
                    emoji = ":white_check_mark:";
                    embed.Color = Color.Green;
                    break;
                case "Contrary":
                    emoji = ":x:";
                    embed.Color = Color.Red;
                    break;
            }

            embed.WithTitle($":8ball: **{question}**");
            embed.WithDescription($"{emoji} {node.ChildNodes[1].InnerText}");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
