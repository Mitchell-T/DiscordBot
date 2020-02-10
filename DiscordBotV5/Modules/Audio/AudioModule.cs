using System.Threading.Tasks;
using Discord;
using Discord.Commands;

public class AudioModule : ModuleBase<ICommandContext>
{
    private readonly AudioService _service;

    public AudioModule(AudioService service)
    {
        _service = service;
    }

    [Command("join", RunMode = RunMode.Async)]
    [Summary("Join your current voice channel")]
    public async Task JoinCmd()
    {
        await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
    }

    [Command("leave", RunMode = RunMode.Async)]
    [Summary("Leave the voice channel")]
    public async Task Leave()
    {
        await _service.LeaveAudio(Context.Guild);
    }

    [Command("play", RunMode = RunMode.Async)]
    [Summary("Play audio from a youtube url")]
    public async Task Play([Remainder] string song)
    {
        await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
    }
}