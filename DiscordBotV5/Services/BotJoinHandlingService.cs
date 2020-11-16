using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotV5.Services
{
    public class BotJoinHandlingService
    {
        private readonly DiscordSocketClient _discord;

        public BotJoinHandlingService(IServiceProvider provider)
        {
            _discord = provider.GetRequiredService<DiscordSocketClient>();
        }


    }
}
