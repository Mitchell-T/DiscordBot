using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using NekosSharp;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord;

namespace DiscordBotV5.Modules.Fun
{
    [Name("Neko")]
    [Summary("Various commands to get Neko images/gifs from nekos.life")]
    [Group("Neko")]
    public class NekoModule : ModuleBase<SocketCommandContext>
    {
        NekoClient _nekoClient;
        public NekoModule()
        {
            _nekoClient = new NekoClient("DiscordbotV5");
            _nekoClient.LogType = LogType.None;
        }

        [Command()]
        public async Task Neko()
        {
            Request req = await _nekoClient.Image_v3.Neko();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("gif")]
        public async Task NekoGif()
        {
            Request req = await _nekoClient.Image_v3.NekoGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("holo")]
        public async Task NekoHolo()
        {
            Request req = await _nekoClient.Image_v3.Holo();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("fox")]
        public async Task NekoFox()
        {
            Request req = await _nekoClient.Image_v3.Fox();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("waifu")]
        public async Task NekoWaifu()
        {
            Request req = await _nekoClient.Image_v3.Waifu();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("wallpaper")]
        public async Task NekoWallpaper()
        {
            Request req = await _nekoClient.Image_v3.Wallpaper();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

    }
}
