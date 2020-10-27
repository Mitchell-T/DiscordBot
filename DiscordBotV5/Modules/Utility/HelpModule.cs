using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.Utility
{
    public class HelpModule : InteractiveBase<SocketCommandContext>
    {
        private readonly CommandService _commands;

        public HelpModule(IServiceProvider provider)
        {
            _commands = provider.GetRequiredService<CommandService>();
        }


        [Command("help")]
        [Summary("Shows a list of all commands and their description")]
        public async Task Help()
        {
            List<CommandInfo> commandList = _commands.Commands.ToList();
            EmbedBuilder embed = new EmbedBuilder();

            foreach (CommandInfo command in commandList)
            {
                embed.AddField(command.Name, command.Summary ?? "No description available\n");
            }



            await ReplyAsync("Here's a list of my commands: ", false, embed.Build());
        }

        [Command("helptest")]
        public async Task HelpTest()
        {
            PaginatedMessage pMsg = new PaginatedMessage();
            pMsg.Content = "**Page 1**\n kill me pls \n kek";
            pMsg.Color = Color.Red;
            var pages = new[] { "**Page 1**\n end me pls \n kek", "Page 2", "Page 3", "aaaaaa", "Page 5" };
            pMsg.Pages = pages;
            await PagedReplyAsync(pMsg);
        }
    }
}
