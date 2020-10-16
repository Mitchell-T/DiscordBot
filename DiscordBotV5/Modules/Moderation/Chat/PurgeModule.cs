using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.Moderation.Chat
{
    public class PurgeModule : ModuleBase<SocketCommandContext>
    {
        [Command("purge")]
        [Summary("purges the chat by x number of messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            // get and delete messages
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages, null);

            // visual readout of howmany messages got deleted
            const int delay = 3000;
            IUserMessage m = await ReplyAsync($"I have deleted {amount} messages");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }
    }
}
