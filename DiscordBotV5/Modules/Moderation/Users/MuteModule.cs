using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.Moderation.Users
{
    public class MuteModule : ModuleBase<SocketCommandContext>
    {

        [Command("mute")]
        [Summary("Mutes a user")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Mute(IGuildUser user)
        {
            if (!user.IsMuted)
                await user.ModifyAsync(x =>
                {
                    x.Mute = true;
                });
        }

        [Command("unmute")]
        [Summary("Unmutes a user")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task UnMute(IGuildUser user)
        {
            if (user.IsMuted)
                await user.ModifyAsync(x =>
                {
                    x.Mute = false;
                });
        }

    }
}
