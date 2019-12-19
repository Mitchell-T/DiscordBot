using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBotV5.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user, [Remainder] string reason = "No reason was provided")
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} kicked for \"{reason}\"");
            await user.KickAsync(reason);
            IDMChannel dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($"You got kicked from {Context.Guild.Name} for \"{reason}\"");
        }


        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, [Remainder] string reason = "No reason was provided")
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} banned for \"{reason}\"");
            await user.KickAsync(reason);
            IDMChannel dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($"You got banned from {Context.Guild.Name} for \"{reason}\"");
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Mute(IGuildUser user)
        {
            if (!user.IsMuted)
            {
                await user.ModifyAsync(x =>
                {
                    x.Mute = true;
                });
            }
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task UnMute(IGuildUser user)
        {
            if (user.IsMuted)
            {
                await user.ModifyAsync(x =>
                {
                    x.Mute = false;
                });
            }
        }


    }
}
