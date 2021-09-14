using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using System;
using Discord.WebSocket;
using DiscordBotV5.Misc;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.Utility
{
    [Name("Info")]
    [Summary("Get various information about the server and users")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;

        public InfoModule(IServiceProvider services)
        {
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
        [RequireUserPermission(GuildPermission.KickMembers)]
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

        [Command("GuildMemberCount")]
        [Alias("gmembercount")]
        [Summary("Show howmany users the bot is watching over")]
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

        [Command("userinfo")]
        [Summary("Show info about a specific user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Info(SocketGuildUser user = null)
        {
            user ??= (SocketGuildUser)Context.User;

            // Get join position
            SocketGuildUser[] sortedMembers = Context.Guild.Users.ToArray().OrderBy(member => member.JoinedAt).ToArray();
            sortedMembers = sortedMembers.Where(val => val.IsBot != true).ToArray(); // Remove bots
            int position = Array.IndexOf(sortedMembers, user) + 1;

            Console.WriteLine(((SocketGuildUser)Context.User).Nickname is null);
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithColor(new Color(47, 49, 54))
                .WithTitle($"User information for: {user.Username + "#" + user.Discriminator}" )
                .AddField("Userinfo:", "**›Username: **" + user.Username + "\n**›Nickname: **" + ((user as IGuildUser).Nickname ?? "No Nickname.") + "\n**›Joined Position: **" + position, true)
                .AddField("‎", "**›Created at: **" + user.CreatedAt.ToString("dd/MM/yyyy") + "\n**›Joined at: **" + (user as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("‎", "**>Roles: **" + string.Join(" ", (user as SocketGuildUser).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp();
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
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