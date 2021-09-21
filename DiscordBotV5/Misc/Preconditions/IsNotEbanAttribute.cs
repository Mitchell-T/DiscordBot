using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Misc.Preconditions
{
    public class IsNotEbanAttribute : PreconditionAttribute
    {
        private readonly List<ulong> _ebans = new List<ulong> { 294359678412914691 };

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(_ebans.Any(x => x == context.User.Id || context.User.Username.ToLower().Contains("eban"))
                ? PreconditionResult.FromError("User is an eban")
                : PreconditionResult.FromSuccess());
        }
    }
}
