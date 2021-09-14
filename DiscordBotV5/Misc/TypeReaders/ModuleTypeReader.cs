using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotV5.Misc.TypeReaders
{
    public class ModuleTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var commandService = services.GetService<CommandService>();
            var module = commandService.Modules.FirstOrDefault(x => x.Name.ToLower() == input.ToLower());

            return Task.FromResult(module != null
                ? TypeReaderResult.FromSuccess(module)
                : TypeReaderResult.FromError(CommandError.ParseFailed, "No module matches the input."));
        }
    }
}