using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.Moderation.Users
{
    public class KickModule : ModuleBase<SocketCommandContext>
    {

        [Command("kick")]
        [Summary("Kick a user from the server")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user, [Remainder] string reason = "No reason was provided")
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} kicked for \"{reason}\"");
            await user.KickAsync(reason);
            IDMChannel dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($"You got kicked from {Context.Guild.Name} for \"{reason}\"");
        }

    }
}
