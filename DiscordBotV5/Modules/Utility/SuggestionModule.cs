using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace DiscordBotV5.Modules.Utility
{
    public class SuggestionModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;

        public SuggestionModule(IConfiguration config)
        {
            _config = config;
        }

        //[Command("suggest")]
        //public async Task Suggest([Remainder] string suggestion)
        //{
        //    await Context.Message.DeleteAsync();

        //    SocketTextChannel suggestionChannel = (SocketTextChannel)Context.Guild.GetChannel(Convert.ToUInt64(_config["suggestionChannelID"]));

        //    IEmote[] emotes = new IEmote[3];
        //    emotes[0] = new Emoji("✅");
        //    emotes[1] = Emote.Parse("<:neutral:680935638832644193>");
        //    emotes[2] = Emote.Parse("<:xmark:680936397313540105>");

        //    var embed = new EmbedBuilder();
        //    embed.WithTitle("New suggestion!");
        //    embed.WithDescription(suggestion);
        //    embed.WithFooter(DateTime.Now.ToShortDateString() + " || Powered by me");
        //    embed.WithColor(new Color(0x03adfc));
        //    IUserMessage suggestionMessage = await suggestionChannel.SendMessageAsync("", false, embed.Build());

        //    await suggestionMessage.AddReactionsAsync(emotes);
        //}
    }
}
