using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotV5.Services;
using Microsoft.Extensions.DependencyInjection;
//using SpotifyAPI.Web;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Search;

[Name("Audio")]
[Summary("Play music in voice channels")]
public class AudioModule : ModuleBase<SocketCommandContext>
{
    private readonly LavaNode _lavaNode;
    //private readonly SpotifyClient _spotifyClient;

    public AudioModule(IServiceProvider provider)
    {
        _lavaNode = provider.GetRequiredService<LavaNode>();
        _lavaNode.OnTrackEnded += OnTrackEnded;
        //_spotifyClient = provider.GetRequiredService<SpotifyService>()._spotifyClient;
    }

    [Command("Join")]
    [Summary("Join the voice channel")]
    public async Task JoinAsync()
    {
        if (_lavaNode.HasPlayer(Context.Guild))
        {
            await ReplyAsync("I'm already connected to a voice channel!");
            return;
        }

        var voiceState = Context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await ReplyAsync("You must be connected to a voice channel!");
            return;
        }

        try
        {
            await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
        }
        catch (Exception exception)
        {
            await ReplyAsync(exception.Message);
        }
    }

    [Command("leave", RunMode = RunMode.Async)]
    [Summary("Leave the voice channel")]
    public async Task Leave()
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        await _lavaNode.LeaveAsync(player.VoiceChannel);
    }

    [Command("Play")]
    [Alias("p")]
    [Summary("Plays a song from Youtube")]
    public async Task PlayAsync([Remainder] string searchQuery)
    {
        //Ensure that the user supplies search terms
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            await ReplyAsync("Please provide search terms.");
            return;
        }

        //Join the voice channel if not already in it
        if (!_lavaNode.HasPlayer(Context.Guild))
        {
            await JoinAsync();
        }

        //Find the search result from the search terms
        var searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, searchQuery);
        if (searchResponse.Status == SearchStatus.LoadFailed ||
            searchResponse.Status == SearchStatus.NoMatches)
        {
            await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.", false);
            return;
        }

        //Get the player and start playing/queueing a single song or playlist
        var player = _lavaNode.GetPlayer(Context.Guild);
        //Single Song
        if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
        {
            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                foreach (var track in searchResponse.Tracks)
                {
                    player.Queue.Enqueue(track);
                }

                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
            }
            else
            {
                var track = searchResponse.Tracks.ElementAt(0);
                player.Queue.Enqueue(track);
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle($"Enqueued: {track.Title}");
                embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
                embed.WithColor(3447003);
                embed.WithDescription(track.Duration.ToString());
                await ReplyAsync("", false, embed.Build());
            }
        }

        //Playlist
        else
        {
            var track = searchResponse.Tracks.ElementAt(0);

            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                for (var i = 0; i < searchResponse.Tracks.Count; i++)
                {
                    if (i == 0)
                    {
                        await player.PlayAsync(track);
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithTitle($"Now Playing: {track.Title}");
                        embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
                        embed.WithColor(3447003);
                        embed.WithDescription(track.Duration.ToString());
                        await ReplyAsync("", false, embed.Build());
                    }
                    else
                    {
                        player.Queue.Enqueue(searchResponse.Tracks.ElementAt(i));
                    }
                }

                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
            }
            else
            {
                await player.PlayAsync(track);
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle($"Now Playing: {track.Title}");
                embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
                embed.WithColor(3447003);
                embed.WithDescription(track.Duration.ToString());
                await ReplyAsync("", false, embed.Build());
            }
        }
    }

    //[Command("SPlay")]
    //[Alias("spotify", "spotifyplay", "spotify-play")]
    //[Summary("Plays a song from Spotify")]
    //public async Task PlaySpotifyAsync([Remainder] string spotifyURL)
    //{
    //    string spotifyID;
    //    try {
    //        string[] splitSpotifyLink = spotifyURL.Split('/');
    //        spotifyID = splitSpotifyLink[4];
    //        spotifyID = spotifyID.Substring(0, spotifyID.IndexOf("?"));
    //    } catch(Exception e)
    //    {
    //        await ReplyAsync("An error occured or given Spotify url is not valid");
    //        return;
    //    }
        

    //    FullTrack spotifyTrack = await _spotifyClient.Tracks.Get(spotifyID);

    //    string searchQuery = (spotifyTrack.Artists.FirstOrDefault().Name.ToString() + " - " + spotifyTrack.Name);
    //    await ReplyAsync(searchQuery);

    //    //Ensure that the user supplies search terms
    //    if (string.IsNullOrWhiteSpace(searchQuery))
    //    {
    //        await ReplyAsync("Please provide search terms.");
    //        return;
    //    }

    //    //Join the voice channel if not already in it
    //    if (!_lavaNode.HasPlayer(Context.Guild))
    //    {
    //        await JoinAsync();
    //    }

    //    //Find the search result from the search terms
    //    var searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, searchQuery);
    //    if (searchResponse.Status == SearchStatus.LoadFailed ||
    //        searchResponse.Status == SearchStatus.NoMatches)
    //    {
    //        await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.", false);
    //        return;
    //    }

    //    //Get the player and start playing/queueing a single song or playlist
    //    var player = _lavaNode.GetPlayer(Context.Guild);
    //    //Single Song
    //    if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
    //    {
    //        if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
    //        {
    //            foreach (var track in searchResponse.Tracks)
    //            {
    //                player.Queue.Enqueue(track);
    //            }

    //            await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
    //        }
    //        else
    //        {
    //            var track = searchResponse.Tracks.ElementAt(0);
    //            player.Queue.Enqueue(track);
    //            EmbedBuilder embed = new EmbedBuilder();
    //            embed.WithTitle($"Enqueued: {track.Title}");
    //            embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
    //            embed.WithColor(3447003);
    //            embed.WithDescription(track.Duration.ToString());
    //            await ReplyAsync("", false, embed.Build());
    //        }
    //    }

    //    //Playlist
    //    else
    //    {
    //        var track = searchResponse.Tracks.ElementAt(0);

    //        if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
    //        {
    //            for (var i = 0; i < searchResponse.Tracks.Count; i++)
    //            {
    //                if (i == 0)
    //                {
    //                    await player.PlayAsync(track);
    //                    EmbedBuilder embed = new EmbedBuilder();
    //                    embed.WithTitle($"Now Playing: {track.Title}");
    //                    embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
    //                    embed.WithColor(3447003);
    //                    embed.WithDescription(track.Duration.ToString());
    //                    await ReplyAsync("", false, embed.Build());
    //                }
    //                else
    //                {
    //                    player.Queue.Enqueue(searchResponse.Tracks.ElementAt(i));
    //                }
    //            }

    //            await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
    //        }
    //        else
    //        {
    //            await player.PlayAsync(track);
    //            EmbedBuilder embed = new EmbedBuilder();
    //            embed.WithTitle($"Now Playing: {track.Title}");
    //            embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
    //            embed.WithColor(3447003);
    //            embed.WithDescription(track.Duration.ToString());
    //            await ReplyAsync("", false, embed.Build());
    //        }
    //    }
    //}

    [Command("Skip")]
    [Summary("Skips the currently playing song")]
    public async Task SkipAsync()
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        EmbedBuilder embed = new EmbedBuilder();
        embed.WithTitle($"Song Skipped: {player.Track.Title}");
        embed.WithThumbnailUrl(player.Track.FetchArtworkAsync().Result);
        embed.WithColor(3447003);
        await ReplyAsync("", false, embed.Build());
        await player.SkipAsync();
    }

    [Command("Skip")]
    [Summary("Skips a specified song in the queue")]
    private async Task RemoveFromQueueAsync([Remainder] int index)
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        if (player.Queue.Count < index)
        {
            await ReplyAsync("Index is longer than the queue length", false);
            return;
        }

        var lavaTrack = player.Queue.ElementAt(index - 1);

        EmbedBuilder embed = new EmbedBuilder();
        embed.WithTitle($"Song Skipped: {lavaTrack.Title}");
        embed.WithThumbnailUrl(lavaTrack.FetchArtworkAsync().Result);
        embed.WithColor(3447003);
        await ReplyAsync("", false, embed.Build());
        player.Queue.RemoveAt(index - 1);
    }

    [Command("Pause")]
    [Summary("Pauses the currently playing song")]
    public async Task PauseSongAsync()
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        await player.PauseAsync();
        await ReplyAsync("Paused");
    }

    [Command("Resume")]
    [Alias("Continue")]
    [Summary("Resume the currently playing song")]
    private async Task ResumeAsync()
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        await player.ResumeAsync();
        await ReplyAsync("Resumed");
    }

    [Command("NowPlaying")]
    [Alias("np")]
    [Summary("Shows the currently playing song")]
    private async Task NowPlayingAsync()
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        EmbedBuilder embed = new EmbedBuilder();
        embed.WithTitle($"Song Skipped: {player.Track.Title}");
        embed.WithThumbnailUrl(player.Track.FetchArtworkAsync().Result);
        embed.WithDescription(player.Track.Position.ToString(@"hh\:mm\:ss") + " / " + player.Track.Duration);
        embed.WithColor(3447003);
        await ReplyAsync("", false, embed.Build());
    }

    [Command("Queue")]
    [Alias("q")]
    [Summary("Shows the current song queue")]
    private async Task QueueAsync()
    {
        var player = _lavaNode.GetPlayer(Context.Guild);
        //Create an embed using that image url
        var builder = new EmbedBuilder();
        builder.WithTitle("Music Queue");
        builder.WithThumbnailUrl(player.Track.FetchArtworkAsync().Result);
        builder.WithColor(3447003);
        builder.WithDescription("");
        TimeSpan totalQueueTime = new TimeSpan();
        foreach (LavaTrack track in player.Queue)
        {
            totalQueueTime += track.Duration;
        }
        builder.WithFooter($"Total queue length: {totalQueueTime}");

        if (player.Queue.Count == 0)
        {
            await ReplyAsync("Queue is empty", false);
            return;
        }

        for (var i = 0; i < player.Queue.Count; i++)
        {
            var lavaTrack = player.Queue.ElementAt(i);
            var fieldBuilder = new EmbedFieldBuilder { Name = $"{i+1} - " + lavaTrack.Title, Value = lavaTrack.Duration };
            builder.AddField(fieldBuilder);
        }

        await ReplyAsync("", false, builder.Build());
    }

    private async Task OnTrackEnded(TrackEndedEventArgs args)
    {
        if (args.Reason != TrackEndReason.Finished)
        {
            return;
        }

        var player = args.Player;
        if (!player.Queue.TryDequeue(out var queueable))
        {
            return;
        }

        if (!(queueable is { } track))
        {
            await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
            return;
        }

        await args.Player.PlayAsync(track);

        EmbedBuilder embed = new EmbedBuilder();
        embed.WithTitle($"Now Playing: {track.Title}");
        embed.WithThumbnailUrl(track.FetchArtworkAsync().Result);
        embed.WithColor(3447003);
        embed.WithDescription(track.Duration.ToString());
        await args.Player.TextChannel.SendMessageAsync("", false, embed.Build());
    }
}