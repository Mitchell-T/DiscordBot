using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using NekosSharp;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord;

namespace DiscordBotV5.Modules.Fun
{
    [Group("Neko")]
    public class NekoModule : ModuleBase<SocketCommandContext>
    {
        NekoClient _nekoClient;
        public NekoModule()
        {
            _nekoClient = new NekoClient("DiscordbotV5");
            _nekoClient.LogType = LogType.None;
        }

        [Command()]
        public async Task Neko()
        {
            Request req = await _nekoClient.Image_v3.Neko();

            if (!req.Success)
            {
                await ReplyAsync("Sorry something went wrong, If this happens more than a few times please message my developer");
                return;
            }

            await Context.Message.DeleteAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Neko");
            embed.WithImageUrl(req.ImageUrl);
            await ReplyAsync("", false, embed.Build());
        }

    }
}
