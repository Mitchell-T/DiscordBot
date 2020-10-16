using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBotV5.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
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

        [Command("mute")]
        [Summary("Mutes a user")]
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

        [Command("unmute")]
        [Summary("Unmutes a user")]
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

        [Command("purge")]
        [Summary("purges the chat by x number of messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            // get and delete messages
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages, null);

            // visual readout of howmany messages got deleted
            const int delay = 3000;
            IUserMessage m = await ReplyAsync($"I have deleted {amount} messages");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }



    }
}
