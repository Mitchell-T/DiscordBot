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
using System.Threading;
using System.Net;
using System.Linq;
using Interactivity;
using DiscordBotV5.Misc.TypeReaders;

namespace DiscordBotV5
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;
        private LavaNode _lavaNode;

        public async Task MainAsync()
        {
            //DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            //{

            //}

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 10000,
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildPresences,
                LogLevel = LogSeverity.Info
            });

            // build or create config
            ConfigTools configBuilder = new ConfigTools();
            _config = configBuilder.Build();

            // Download required files from my server
            PrerequisiteFilesDownloader.StartupCheck();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            services.GetRequiredService<DatabaseService>().Initialize();
            services.GetRequiredService<ServerPreferenceService>().Initialize();
            //services.GetRequiredService<SpotifyService>().Initialize();



            _lavaNode = services.GetRequiredService<LavaNode>();

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
            var commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Error,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                IgnoreExtraArgs = false,
            });

            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                //.AddSingleton<CommandService>()
                .AddSingleton(services =>
                {
                    commandService.AddTypeReader<ModuleInfo>(new ModuleTypeReader());
                    return commandService;
                })
                .AddSingleton<CommandHandlingService>()
                // Audio
                .AddSingleton<AudioService>()
                .AddLavaNode(x =>
                {
                    x.SelfDeaf = false;
                    x.Authorization = "doesntMatterisLocal";
                })
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Config
                .AddSingleton(_config)
                // Database
                .AddSingleton<DatabaseService>()
                // Server preferences
                .AddSingleton<ServerPreferenceService>()
                // Server join handler
                .AddSingleton<BotJoinHandlingService>()
                // Add additional services here...
                .AddSingleton<InteractivityService>()
                .AddSingleton(new InteractivityConfig { DefaultTimeout = TimeSpan.FromSeconds(20) })
                //.AddSingleton<SpotifyService>()
                // DONE
                .BuildServiceProvider();
        }

        private async Task OnClientReady()
        {
            // Set the bots status
            await _client.SetActivityAsync(new Game($"commands in {_client.Guilds.Count} guilds || $help", ActivityType.Listening, ActivityProperties.None));

            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
            Console.WriteLine(_lavaNode.IsConnected);


            // Cache guild members upon startup
            _ = Task.Run(async () =>
              {
                  Console.WriteLine("loading guilds....");
                  await Task.WhenAll(_client.Guilds.Select(g => g.DownloadUsersAsync()));
                  int count = _client.Guilds.Sum(g => g.Users.Count);
                  Console.WriteLine($"done loading guilds, cached {count} users");
              });
            
        }

    }

}