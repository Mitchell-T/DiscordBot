using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Misc.Preconditions
{
    public class RequireDeveloperAttribute : PreconditionAttribute
    {
        private readonly List<ulong> _developers = new List<ulong> { 165846891941199872 };

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(_developers.Any(x => x == context.User.Id)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("User is not a developer"));
        }
    }
}
