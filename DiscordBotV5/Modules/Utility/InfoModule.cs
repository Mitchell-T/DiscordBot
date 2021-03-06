﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using System;
using Discord.WebSocket;
using DiscordBotV5.Misc;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace DiscordBot.Modules.Utility
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;

        public InfoModule(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
        }

        [Command("info")]
        [Summary("Show info about the bot")]
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
        [Summary("Show info about the server")]
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
            embed.AddField("Server age", TimeTools.PeriodOfTimeOutput(DateTime.Now.Subtract(Context.Guild.CreatedAt.DateTime)), true);
            embed.WithFooter($"Server ID: {Context.Guild.Id} | Server created: {Context.Guild.CreatedAt.ToString()}");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("membercount")]
        [Summary("Show howmany members your server has")]
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

        [Command("gmembercount")]
        [Summary("Show howmany members the bot is in")]
        public async Task GMemberCount()
        {
            int totalMembers = 0;
            foreach (SocketGuild guild in _client.Guilds)
            {
                totalMembers += guild.MemberCount;
            }

            var embed = new EmbedBuilder();
            embed.AddField("Members", totalMembers, true);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("whois")]
        [Summary("Show info about a specific user")]
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

            SocketGuildUser[] sortedMembers = Context.Guild.Users.ToArray().OrderBy(member => member.JoinedAt).ToArray();

            // Remove bots from array
            sortedMembers = sortedMembers.Where(val => val.IsBot != true).ToArray();

            int position = Array.IndexOf(sortedMembers, userToCheck) + 1;
            //DateTimeFormatInfo mfi = new DateTimeFormatInfo();

            //DateTimeOffset createdAt = userToCheck.CreatedAt;
            //DateTimeOffset joinedAt = (DateTimeOffset)userToCheck.JoinedAt;
            //string createdOn = $"{mfi.GetMonthName(createdAt.Month)} {createdAt.Day}th {createdAt.Year}, {createdAt.ToString("hh:mm:ss")} - {(int)(DateTime.Now - createdAt).TotalDays} days";
            //string boostingSince = "Not Boosting";

            //if (userToCheck.PremiumSince != null)
            //{
            //    ((DateTimeOffset)userToCheck.PremiumSince).ToString("mm/dd/yy");
            //}

            var embed = new EmbedBuilder();
            embed.WithTitle("WhoIs Lookup for : " + userToCheck.Username);
            embed.WithThumbnailUrl(userToCheck.GetAvatarUrl());
            embed.WithDescription($"**User :** {userToCheck}\n" +
                                  $"**Nickname :** {nickName}\n" +
                                  $"**Created on :** {userToCheck.CreatedAt}\n" +
                                  $"**Joined server on :** {userToCheck.JoinedAt}\n" +
                                  $"**Join position : ** {position}\n" +
                                  $"**Current Game :** {gamePlaying}\n" +
                                  $"**Owner :** {isOwner}\n");
            embed.WithColor(new Color(102, 255, 125));

            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }

        [Command("ping")]
        [Summary("Show the bots ping to discord servers")]
        public async Task Ping()
        {
            int ping = Context.Client.Latency;
            var embed = new EmbedBuilder();
            Color statusColor;
            if(ping <= 100)
            {
                statusColor = Color.Green;
            } else if(ping < 200 && ping > 100)
            {
                statusColor = Color.Orange;
            } else
            {
                statusColor = Color.Red;
            }
            embed.WithColor(statusColor);
            embed.WithDescription($":ping_pong: pong!, my ping is currently {ping}");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}