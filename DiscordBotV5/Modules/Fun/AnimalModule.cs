using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Xml;
using RestSharp;

namespace DiscordBotV5.Modules.Fun
{
    public class AnimalModule : ModuleBase<SocketCommandContext>
    {

        [Command("cat")]
        public async Task Cat(string catType = "gif")
        {
            await Context.Message.DeleteAsync();

            string catLocation = "http://thecatapi.com/api/images/get?format=xml&results_per_page=1&type=" + catType + "&api_key=MzM0NDcw";
             
            XmlDocument catXML = new XmlDocument();
            catXML.Load(catLocation);
            XmlElement rootCat = catXML.DocumentElement;
            XmlNode urlnode = rootCat.SelectSingleNode("//url");

            var embed = new EmbedBuilder();
            embed.WithTitle("Cat requested by: " + Context.User.Username);
            embed.WithImageUrl(urlnode.InnerXml);
            embed.WithColor(new Color(102, 255, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }


        [Command("dog")]
        public async Task Dog(string dogType = "gif")
        {
            await Context.Message.DeleteAsync();

            var client = new RestClient("https://dog.ceo/api/breeds/image/random");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            string dogImgURL = response.Content.Substring(12);
            dogImgURL = dogImgURL.Substring(0, dogImgURL.IndexOf("\""));
            dogImgURL = dogImgURL.Replace(@"\", String.Empty);

            var embed = new EmbedBuilder();
            embed.WithTitle("Dog requested by: " + Context.User.Username);
            embed.WithImageUrl(dogImgURL);
            embed.WithColor(new Color(102, 255, 255));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

    }
}
