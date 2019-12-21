using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using System;
using Discord.WebSocket;
using System.Collections.Generic;

namespace DiscordBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [Alias("i")]
        public async Task Info()
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder();
            embed.WithTitle("Bot Info");
            embed.WithColor(Color.Gold);
            embed.WithDescription($"Hello, My name is {Context.Client.CurrentUser.Username}. I am written in C#(Discord.net) by <@165846891941199872>\n" +
                "My source code is available on [Github](https://github.com/MLGC00KIE/DiscordBot)\n");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("serverinfo")]
        [Alias("si")]
        public async Task ServerInfo()
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder();
            embed.WithAuthor(Context.Guild.Name, Context.Guild.IconUrl);
            embed.WithColor(Color.DarkRed);
            embed.AddField("Owner", Context.Guild.Owner.Mention);
            embed.AddField("Channels", Context.Guild.Channels.Count, true);
            embed.AddField("Text channels", Context.Guild.TextChannels.Count, true);
            embed.AddField("Voice channels", Context.Guild.VoiceChannels.Count, true);
            embed.AddField("Categories", Context.Guild.CategoryChannels.Count);
            embed.AddField("Members", Context.Guild.MemberCount, true);
            embed.AddField("Ban count", (await Context.Guild.GetBansAsync()).Count, true);
            embed.AddField("Roles", Context.Guild.Roles.Count, true);
            embed.WithFooter($"Server ID: {Context.Guild.Id} | Server created: {Context.Guild.CreatedAt.ToString()}");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("membercount")]
        [Alias("mc")]
        public async Task MemberCount()
        {
            int botCount = 0;
            int humanCount = 0;
            foreach(IGuildUser user in Context.Guild.Users)
            {
                if (!user.IsBot)
                {
                    humanCount++;
                }
                else
                {
                    botCount++;
                }
            }

            var embed = new EmbedBuilder();
            embed.AddField("Members", Context.Guild.MemberCount, true);
            embed.AddField("Humans", humanCount, true);
            embed.AddField("Bots", botCount, true);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("whois")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task WhoIs(IGuildUser userToCheck)
        {
            string gamePlaying = userToCheck.Activity?.Name ?? "Not playing any games";
            string nickName = userToCheck.Nickname ?? "-";
            var owner = Context.Guild.Owner;
            string isOwner;

            if (userToCheck == owner)
            {
                isOwner = "true";
            }
            else
            {
                isOwner = "false";
            }

            SocketGuildUser[] sortedMembers = Context.Guild.Users.ToArray().OrderBy(a => a.JoinedAt).ToArray();

            int position = Array.IndexOf(sortedMembers, userToCheck) + 1;


            var embed = new EmbedBuilder();
            embed.WithTitle("WhoIs Lookup for : " + userToCheck.Username);
            embed.WithThumbnailUrl(userToCheck.GetAvatarUrl());
            embed.WithDescription("**User :** " + userToCheck + "\n" +
                                  "**Nickname :** " + nickName + "\n" +
                                  "**Created on :** " + userToCheck.CreatedAt + "\n" +
                                  "**Joined server on :** " + userToCheck.JoinedAt + "\n" +
                                  "**Join position : **" + position + "\n" +
                                  "**Current Game :** " + gamePlaying + "\n" +
                                  "**Owner :** " + isOwner + "\n");
            embed.WithColor(new Color(102, 255, 125));

            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }
    }
}