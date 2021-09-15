using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordBotV5.Services
{
    public sealed class AudioService
    {
        private readonly LavaNode _lavaNode;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

        public AudioService(LavaNode lavaNode, ILoggerFactory loggerFactory)
        {
            Console.WriteLine("AudioService loaded");

            _lavaNode = lavaNode;
            _logger = loggerFactory.CreateLogger<LavaNode>();
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();

            _lavaNode.OnLog += arg => {
                if ((int)arg.Severity <= 2)
                    _logger.Log(LogLevel.Information, arg.Exception, arg.Message);

                return Task.CompletedTask;
            };

            _lavaNode.OnPlayerUpdated += OnPlayerUpdated;
            _lavaNode.OnStatsReceived += OnStatsReceived;
            _lavaNode.OnTrackEnded += OnTrackEnded;
            _lavaNode.OnTrackStarted += OnTrackStarted;
            _lavaNode.OnTrackException += OnTrackException;
            _lavaNode.OnTrackStuck += OnTrackStuck;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosed;

        }

        private Task OnPlayerUpdated(PlayerUpdateEventArgs arg)
        {
            _logger.LogInformation($"Track update received for {arg.Track.Title}: {arg.Position}");
            return Task.CompletedTask;
        }

        private Task OnStatsReceived(StatsEventArgs arg)
        {
            _logger.LogInformation($"Lavalink has been up for {arg.Uptime}.");
            return Task.CompletedTask;
        }

        private async Task OnTrackStarted(TrackStartEventArgs arg)
        {
            Console.WriteLine("started");
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"Now Playing: {arg.Track.Title}");
            embed.WithThumbnailUrl(arg.Track.FetchArtworkAsync().Result);
            embed.WithColor(3447003);
            embed.WithDescription(arg.Track.Duration.ToString());
            await arg.Player.TextChannel.SendMessageAsync("", false, embed.Build());

            if (!_disconnectTokens.TryGetValue(arg.Player.VoiceChannel.Id, out var value))
            {
                return;
            }

            if (value.IsCancellationRequested)
            {
                return;
            }

            value.Cancel(true);
            await arg.Player.TextChannel.SendMessageAsync("Auto disconnect has been cancelled!");
        }

        private async Task OnTrackEnded(TrackEndedEventArgs args)
        {

            Console.WriteLine("ended");
            if (args.Reason != TrackEndReason.Finished)
            {
                return;
            }

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var lavaTrack))
            {
                await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks to rock n' roll!");
                _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromSeconds(10));
                return;
            }

            if (lavaTrack is null)
            {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(lavaTrack);
        }

        private async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan)
        {
            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value))
            {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }
            else if (value.IsCancellationRequested)
            {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            await player.TextChannel.SendMessageAsync($"Auto disconnect initiated! Disconnecting in {timeSpan}...");
            var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
            if (isCancelled)
            {
                return;
            }

            await _lavaNode.LeaveAsync(player.VoiceChannel);
            await player.TextChannel.SendMessageAsync("Invite me again sometime, sugar.");
        }

        private async Task OnTrackException(TrackExceptionEventArgs arg)
        {
            _logger.LogError($"Track {arg.Track.Title} threw an exception. Please check Lavalink console/logs.");
            arg.Player.Queue.Enqueue(arg.Track);
            await arg.Player.TextChannel.SendMessageAsync(
                $"{arg.Track.Title} has been re-added to queue after throwing an exception.");
        }

        private async Task OnTrackStuck(TrackStuckEventArgs arg)
        {
            _logger.LogError(
                $"Track {arg.Track.Title} got stuck for {arg.Threshold}ms. Please check Lavalink console/logs.");
            arg.Player.Queue.Enqueue(arg.Track);
            await arg.Player.TextChannel.SendMessageAsync(
                $"{arg.Track.Title} has been re-added to queue after getting stuck.");
        }

        private Task OnWebSocketClosed(WebSocketClosedEventArgs arg)
        {
            _logger.LogCritical($"Discord WebSocket connection closed with following reason: {arg.Reason}");
            return Task.CompletedTask;
        }
    }
}