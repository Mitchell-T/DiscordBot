using Discord;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DiscordBotV5.Services.Help;
using Interactivity;
using DiscordBotV5.Services.Attributes;
using Interactivity.Pagination;

namespace DiscordBotV5.Modules.Utility
{
    [Name("Help")]
    [Summary("bot handbook")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly InteractivityService _interactivity;

        public HelpModule(IServiceProvider provider)
        {
            _commands = provider.GetRequiredService<CommandService>();
            _interactivity = provider.GetRequiredService<InteractivityService>();
        }

        [Command("Help", RunMode = RunMode.Async)]
        [Summary("all my commands")]
        public async Task helpAsync()
        {
            var modules = _commands.Modules;
            var pages = new List<PageBuilder>();

            foreach (var module in modules)
            {
                var pageFields = new List<EmbedFieldBuilder>();

                foreach(CommandInfo command in module.Commands)
                {
                    EmbedFieldBuilder embedField = new EmbedFieldBuilder();
                    embedField.WithName(HelpUtilities.GetCommandUsage(command));
                    embedField.WithValue(command.Summary ?? "no information given");
                    pageFields.Add(embedField);
                }

                if(pageFields.Count <= 25)
                {
                    PageBuilder page = new PageBuilder();
                    page.WithTitle(module.Name);
                    page.WithFields(pageFields);
                    pages.Add(page);
                }
                else
                {
                    while(pageFields.Count > 0)
                    {
                        PageBuilder page = new PageBuilder();
                        page.WithTitle(module.Name);
                        page.WithFields(pageFields.Take(25));
                        pages.Add(page);
                        if(pageFields.Count > 25)
                        {
                            pageFields.RemoveRange(0, 24);
                        }
                        else
                        {
                            pageFields.Clear();
                        }
                    }
                }

                
            }
            var paginator = new StaticPaginatorBuilder()
                .WithUsers(Context.User)
                .WithPages(pages)
                .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
                .WithDefaultEmotes()
                .Build();

            await _interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
        }

        [Command("help")]
        [Summary("Module specific commands")]
        public async Task Help(ModuleInfo module)
        {
            var pages = new List<PageBuilder>();
            var pageFields = new List<EmbedFieldBuilder>();

            foreach (CommandInfo command in module.Commands)
            {
                EmbedFieldBuilder embedField = new EmbedFieldBuilder();
                embedField.WithName(HelpUtilities.GetCommandUsage(command));
                embedField.WithValue(command.Summary ?? "no information given");
                pageFields.Add(embedField);
            }

            if (pageFields.Count <= 25)
            {
                PageBuilder page = new PageBuilder();
                page.WithTitle(module.Name);
                page.WithFields(pageFields);
                pages.Add(page);

                var builder = new EmbedBuilder();
                builder.WithTitle(module.Name);
                builder.WithFields(pageFields);
                await ReplyAsync("", false, builder.Build());

            }
            else
            {
                while (pageFields.Count > 0)
                {
                    PageBuilder page = new PageBuilder();
                    page.WithTitle(module.Name);
                    page.WithFields(pageFields.Take(25));
                    pages.Add(page);
                    if (pageFields.Count > 25)
                    {
                        pageFields.RemoveRange(0, 24);
                    }
                    else
                    {
                        pageFields.Clear();
                    }
                }

                var paginator = new StaticPaginatorBuilder()
                .WithUsers(Context.User)
                .WithPages(pages)
                .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
                .WithDefaultEmotes()
                .Build();

                await _interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
            }
        }
    }
}
