using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.Moderation.Users
{
    public class BanModule : ModuleBase<SocketCommandContext>
    {

        [Command("ban")]
        [Summary("Ban a user from the server")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, [Remainder] string reason = "No reason was provided")
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} banned for \"{reason}\"");
            await user.KickAsync(reason);
            IDMChannel dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($"You got banned from {Context.Guild.Name} for \"{reason}\"");
        }

    }
}
