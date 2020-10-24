using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System;
using System.Xml;
using RestSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotV5.Modules.Fun
{
    public class AnimalModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;

        public AnimalModule(IServiceProvider provider)
        {
            _config = provider.GetRequiredService<IConfiguration>();
        }

        [Command("cat")]
        [Summary("Post a cute cat picture")]
        public async Task Cat(string catType = "gif")
        {
            await Context.Message.DeleteAsync();

            string catLocation = "http://thecatapi.com/api/images/get?format=xml&results_per_page=1&type=" + catType + "&api_key=" + _config["TheCatApiKey"];
             
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

        [Command("catBomb")]
        [Summary("Spam the chat with cute kitties")]
        public async Task CatBomb()
        {
            await Context.Message.DeleteAsync();

            int amount = 5;

            string catLocation = $"http://thecatapi.com/api/images/get?format=xml&results_per_page=" + amount + "&type=" + "gif" + "&api_key=" + _config["TheCatApiKey"];

            XmlDocument catXML = new XmlDocument();
            catXML.Load(catLocation);
            XmlElement rootCat = catXML.DocumentElement;
            XmlNodeList urlnodes = rootCat.SelectNodes("//url");

            Random rnd = new Random();

            foreach (XmlNode node in urlnodes)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Cat requested by: " + Context.User.Username);
                embed.WithImageUrl(node.InnerXml);
                embed.WithColor(new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255)));

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }


        [Command("dog")]
        [Summary("Post a cute doggo picture")]
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

        [Command("dogbomb")]
        [Summary("Spam the chat with cute doggos")]
        public async Task DogBomb()
        {
            await Context.Message.DeleteAsync();

            Random rnd = new Random();

            for (int i = 0; i < 5; i++)
            {
                var client = new RestClient("https://dog.ceo/api/breeds/image/random");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                string dogImgURL = response.Content.Substring(12);
                dogImgURL = dogImgURL.Substring(0, dogImgURL.IndexOf("\""));
                dogImgURL = dogImgURL.Replace(@"\", String.Empty);

                var embed = new EmbedBuilder();
                embed.WithTitle("Dog requested by: " + Context.User.Username);
                embed.WithImageUrl(dogImgURL);
                embed.WithColor(new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255)));

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }         

        }

    }
}
