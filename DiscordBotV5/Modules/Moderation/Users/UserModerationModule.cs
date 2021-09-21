using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.Moderation.Users
{
    [Name("UserModeration")]
    [Summary("Various moderation actions for users")]
    public class UserModerationModule : ModuleBase<SocketCommandContext>
    {
        [Command("mute")]
        [Summary("Mutes a user in a voice channel")]
        //[RequireUserPermission(GuildPermission.MuteMembers)]
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
        [Summary("Unmutes a user in a voice channel")]
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

        [Command("kick")]
        [Summary("Kick a user from the server")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user, [Remainder] string reason = "No reason was provided")
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} kicked for \"{reason}\"");
            await user.KickAsync(reason);
            try { await user.SendMessageAsync($"You got kicked from {Context.Guild.Name} for \"{reason}\""); }
            catch (Discord.Net.HttpException ex) when ((int)ex.HttpCode == 50007) { }

        }

        [Command("ban")]
        [Summary("Ban a user from the server")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, [Remainder] string reason = "No reason was provided")
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} banned for \"{reason}\"");
            await user.KickAsync(reason);
            try { await user.SendMessageAsync($"You got banned from {Context.Guild.Name} for \"{reason}\""); }
            catch (Discord.Net.HttpException ex) when ((int)ex.HttpCode == 50007) { }
        }
    }
}
