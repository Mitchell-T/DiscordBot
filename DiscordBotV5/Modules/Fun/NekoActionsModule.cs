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
    [Name("Neko Actions")]
    [Summary("Various neko actions from nekos.life")]
    public class NekoActionsModule : ModuleBase<SocketCommandContext>
    {
        NekoClient _nekoClient;
        public NekoActionsModule()
        {
            _nekoClient = new NekoClient("DiscordbotV5");
            _nekoClient.LogType = LogType.None;
        }

        [Command("slap")]
        public async Task NekoSlap(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.SlapGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** slaps **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            embed.WithCurrentTimestamp();
            await ReplyAsync("", false, embed.Build());
        }

        [Command("poke")]
        public async Task NekoPoke(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.PokeGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** pokes **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("hug")]
        public async Task NekoHug(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.HugGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** is getting hugged by **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("kiss")]
        public async Task NekoKiss(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.KissGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** kisses **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("pat")]
        public async Task NekoPat(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.PatGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** pats **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("tickle")]
        public async Task NekoTickle(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.TickleGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** tickles **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("feed")]
        public async Task NekoFeed(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.FeedGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** feeds **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("cuddle")]
        public async Task NekoCuddle(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;

            Request req = await _nekoClient.Action_v3.CuddleGif();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"**{Context.User.Mention}** cuddles **{user.Mention}**");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }



    }
}
