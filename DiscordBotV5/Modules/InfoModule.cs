using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;

namespace DiscordBot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
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
    }
}