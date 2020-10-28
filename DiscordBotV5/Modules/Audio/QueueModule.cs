using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordBotV5.Modules.Audio
{
    public class QueueModule : ModuleBase<SocketCommandContext>
    {

        [Command("queue")]
        public async Task Queue()
        {




            var embed = new EmbedBuilder();
            embed.WithTitle("Queue:");
        }

    }
}
