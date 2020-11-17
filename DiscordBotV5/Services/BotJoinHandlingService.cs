using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DiscordBotV5.Services
{
    public class BotJoinHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly ServerPreferenceService _preferences;

        public BotJoinHandlingService(IServiceProvider provider)
        {
            _discord = provider.GetRequiredService<DiscordSocketClient>();
            _preferences = provider.GetRequiredService<ServerPreferenceService>();
        }

        public void Initialize()
        {
            _discord.JoinedGuild += HandleJoinAsync;
        }

        public async Task HandleJoinAsync(SocketGuild guild)
        {
            _preferences.OnServerJoin(guild);
        }
    }


}
