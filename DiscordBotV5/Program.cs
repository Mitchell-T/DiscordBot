using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBotV5.Misc;
using DiscordBotV5.Services;
using Victoria;

namespace DiscordBotV5
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            // build or create config
            ConfigTools configBuilder = new ConfigTools();
            _config = configBuilder.Build();

            // Download required files from my server
            PrerequisiteFilesDownloader.StartupCheck();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            services.GetRequiredService<DatabaseService>().Initialize();

            if (_config["token"] == "insert token here")
            {
                Console.WriteLine("NO TOKEN SET IN CONFIG FILE");
                Environment.Exit(0);
            }

            // log in the bot with the supplied bot token
            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            _client.Ready += OnClientReady;

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Audio
                .AddSingleton<AudioService>()
                .AddLavaNode(x =>
                {
                    x.SelfDeaf = false;
                })
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Config
                .AddSingleton(_config)
                // Database
                .AddSingleton<DatabaseService>()
                // Add additional services here...

                // DONE
                .BuildServiceProvider();
        }

        private async Task OnClientReady()
        {
            // set the bots status
            await _client.SetActivityAsync(new Game($"commands in {_client.Guilds.Count} guilds || $help", ActivityType.Listening, ActivityProperties.None));

            //if (!_instanceOfLavaNode.IsConnected)
            //{
            //    _instanceOfLavaNode.ConnectAsync();
            //}
        }

    }
}