using Discord.Commands;
using DiscordBotV5.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Modules.BotManagement
{
    [RequireOwner]
    public class ManagePreferenceModule : ModuleBase<SocketCommandContext>
    {
        private ServerPreferenceService _preferences;
        
        public ManagePreferenceModule(IServiceProvider provider)
        {
            _preferences = provider.GetRequiredService<ServerPreferenceService>();
        }

        [Command("resetpreferencecache")]
        public async Task ResetPreferenceCache()
        {
            _preferences.ResetCache();
            await ReplyAsync("Cache reset!");
        }

    }
}
