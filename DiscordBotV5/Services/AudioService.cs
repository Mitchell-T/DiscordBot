﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
            return;

        if (target.Guild.Id != guild.Id)
            return;

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient)){}
    }

    public async Task LeaveAudio(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
            await client.StopAsync();
    }

    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
    {

        if (Environment.OSVersion.Platform == PlatformID.Unix)
            return;

        //if (!File.Exists(path))
        //{
        //    await channel.SendMessageAsync("File does not exist.");
        //    return;
        //}
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            Console.WriteLine(path);
            Console.WriteLine(Directory.GetCurrentDirectory());
            using (var ffmpeg = CreateProcess(path))
            using (var stream = client.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }
        }
    }

    private Process CreateProcess(string path)
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Unix:
                return CreateUnixProcess(path);

        }
        return Process.Start(new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = $"/C youtube-dl -o - {path} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = false
        }); ;
    }

    private Process CreateUnixProcess(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"/C youtube-dl -o - {path} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = false
        }); ;
    }
}