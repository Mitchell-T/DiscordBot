using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deprived.Modules
{
    public class Userinfo : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        public async Task Info(SocketGuildUser user = null)
        {
            user ??= (SocketGuildUser)Context.User;

            Console.WriteLine(((SocketGuildUser)Context.User).Nickname is null);
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithColor(new Color(47, 49, 54))
                .AddField("Userinfo:", "**›Username: **" + user.Username + "\n**›Nickname: **" + ((user as IGuildUser).Nickname ?? "No Nickname.") + "\n**›Joined Position: **" + "To be added" + "\n**›Bot: **" + "To be added", true)
                .AddField("‎", "**›Created at: **" + user.CreatedAt.ToString("dd/MM/yyyy") + "\n**›Joined at: **" + (user as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("‎", "**>Roles: **" + string.Join(" ", (user as SocketGuildUser).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp();
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}
 