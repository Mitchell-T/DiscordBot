using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotV5.Services;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Search;

[Name("Audio")]
[Summary("Play music in voice channels")]
public class AudioModule : ModuleBase<SocketCommandContext>
{
    private readonly LavaNode _lavaNode;
    private readonly SpotifyClient _spotifyClient;
    private readonly AudioService _audioService;

    private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

    public AudioModule(IServiceProvider provider, LavaNode lavaNode, SpotifyService spotifyService, AudioService audioService)
    {
        _lavaNode = lavaNode;
        _spotifyClient = spotifyService._spotifyClient;
        _audioService = audioService;
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

        var searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, searchQuery);
        if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
        {
            await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.");
            return;
        }

        var player = _lavaNode.GetPlayer(Context.Guild);
        if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
        {
            player.Queue.Enqueue(searchResponse.Tracks);
            await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} songs.");
        }
        else
        {
            var track = searchResponse.Tracks.FirstOrDefault();
            player.Queue.Enqueue(track);

            await ReplyAsync($"Enqueued {track?.Title}");
        }

        if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
        {
            return;
        }

        player.Queue.TryDequeue(out var lavaTrack);
        await player.PlayAsync(x => {
            x.Track = lavaTrack;
            x.ShouldPause = false;
        });
    }

    [Command("SPlay")]
    [Alias("spotify", "spotifyplay", "spotify-play")]
    [Summary("Plays a song from Spotify")]
    public async Task PlaySpotifyAsync([Remainder] string spotifyURL)
    {
        string spotifyID;
        try
        {
            string[] splitSpotifyLink = spotifyURL.Split('/');
            spotifyID = splitSpotifyLink[4];
            spotifyID = spotifyID.Substring(0, spotifyID.IndexOf("?"));
        }
        catch (Exception e)
        {
            await ReplyAsync("An error occured or given Spotify url is not valid " + e.Message);
            return;
        }


        FullTrack spotifyTrack = await _spotifyClient.Tracks.Get(spotifyID);

        string searchQuery = (spotifyTrack.Artists.FirstOrDefault().Name.ToString() + " - " + spotifyTrack.Name);

        //Join the voice channel if not already in it
        if (!_lavaNode.HasPlayer(Context.Guild))
        {
            await JoinAsync();
        }

        var searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, searchQuery);
        if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
        {
            await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.");
            return;
        }

        var player = _lavaNode.GetPlayer(Context.Guild);
        if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
        {
            player.Queue.Enqueue(searchResponse.Tracks);
            await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} songs.");
        }
        else
        {
            var track = searchResponse.Tracks.FirstOrDefault();
            player.Queue.Enqueue(track);

            await ReplyAsync($"Enqueued {track?.Title}");
        }

        if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
        {
            return;
        }

        player.Queue.TryDequeue(out var lavaTrack);
        await player.PlayAsync(x => {
            x.Track = lavaTrack;
            x.ShouldPause = false;
        });
    }

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
    public async Task PauseAsync()
    {
        if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await ReplyAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await ReplyAsync("I cannot pause when I'm not playing anything!");
            return;
        }

        try
        {
            await player.PauseAsync();
            await ReplyAsync($"Paused: {player.Track.Title}");
        }
        catch (Exception exception)
        {
            await ReplyAsync(exception.Message);
        }
    }

    [Command("Resume")]
    [Alias("Continue")]
    [Summary("Resume the currently playing song")]
    public async Task ResumeAsync()
    {
        if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await ReplyAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Paused)
        {
            await ReplyAsync("I cannot resume when I'm not playing anything!");
            return;
        }

        try
        {
            await player.ResumeAsync();
            await ReplyAsync($"Resumed: {player.Track.Title}");
        }
        catch (Exception exception)
        {
            await ReplyAsync(exception.Message);
        }
    }

    [Command("Stop")]
    [Summary("Stops playing audio")]
    public async Task StopAsync()
    {
        if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await ReplyAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState == PlayerState.Stopped)
        {
            await ReplyAsync("Woaaah there, I can't stop the stopped forced.");
            return;
        }

        try
        {
            await player.StopAsync();
            await ReplyAsync("No longer playing anything.");
        }
        catch (Exception exception)
        {
            await ReplyAsync(exception.Message);
        }
    }

    [Command("Seek")]
    [Summary("Seek through a song")]
    public async Task SeekAsync([Remainder] string seekString)
    {
        if(!TimeSpan.TryParse(seekString, out TimeSpan timeSpan))
        {
            await ReplyAsync("Input is not a valid timestamp");
            return;
        }
            
        if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await ReplyAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await ReplyAsync("Woaaah there, I can't seek when nothing is playing.");
            return;
        }

        try
        {
            await player.SeekAsync(timeSpan);
            await ReplyAsync($"I've seeked `{player.Track.Title}` to {timeSpan}.");
        }
        catch (Exception exception)
        {
            await ReplyAsync(exception.Message);
        }
    }

    [Command("Genius", RunMode = RunMode.Async)]
    [Summary("Query Genius for the song lyrics")]
    public async Task ShowGeniusLyrics()
    {
        if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await ReplyAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await ReplyAsync("Woaaah there, I'm not playing any tracks.");
            return;
        }

        var lyrics = await player.Track.FetchLyricsFromGeniusAsync();
        if (string.IsNullOrWhiteSpace(lyrics))
        {
            await ReplyAsync($"No lyrics found for {player.Track.Title}");
            return;
        }

        await SendLyricsAsync(lyrics);
    }

    [Command("OVH", RunMode = RunMode.Async)]
    [Summary("Query OVH for the song lyrics")]
    public async Task ShowOvhLyrics()
    {
        if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await ReplyAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await ReplyAsync("Woaaah there, I'm not playing any tracks.");
            return;
        }

        var lyrics = await player.Track.FetchLyricsFromOvhAsync();
        if (string.IsNullOrWhiteSpace(lyrics))
        {
            await ReplyAsync($"No lyrics found for {player.Track.Title}");
            return;
        }

        await SendLyricsAsync(lyrics);
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

    private async Task SendLyricsAsync(string lyrics)
    {
        var splitLyrics = lyrics.Split(Environment.NewLine);
        var stringBuilder = new StringBuilder();
        foreach (var line in splitLyrics)
        {
            if (line.Contains('['))
            {
                stringBuilder.Append(Environment.NewLine);
            }

            if (Range.Contains(stringBuilder.Length))
            {
                await ReplyAsync($"```{stringBuilder}```");
                stringBuilder.Clear();
            }
            else
            {
                stringBuilder.AppendLine(line);
            }
        }

        await ReplyAsync($"```{stringBuilder}```");
    }

}