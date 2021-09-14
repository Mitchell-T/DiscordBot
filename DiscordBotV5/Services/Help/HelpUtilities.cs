using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using DiscordBotV5.Services.Attributes;
using MoreLinq;

namespace DiscordBotV5.Services.Help
{
    public static class HelpUtilities
    {
        public static string GetCommandUsage(CommandInfo command)
        {
            var sb = new StringBuilder().Append(command.Name);

            if (command.Parameters.Any())
            {
                foreach (var param in command.Parameters)
                {
                    sb.Append(!param.IsOptional
                        ? $" [{param.Name}]"
                        : $" <{param.Name}>");
                }
            }

            return sb.ToString();
        }
    }
}