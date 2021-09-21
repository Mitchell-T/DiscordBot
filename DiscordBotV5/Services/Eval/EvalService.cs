using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DiscordBotV5.Services.Eval
{
    public class EvalService : IEvalService
    {

        private readonly DiscordSocketClient _client;
        private readonly ScriptOptions _scriptOptions;

        public EvalService()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var imports = assemblies.SelectMany(x => x.GetTypes())
                .Select(x => x.Namespace)
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x));

            _scriptOptions = ScriptOptions.Default
                .WithImports(imports)
                .WithReferences(assemblies.Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));
        }

        //private readonly ScriptOptions _scriptOptions = _scriptOptions.AddReferences(
        //        typeof (object).Assembly,
        //        typeof (Enumerable).Assembly,
        //        _client.GetType().Assembly)
        //        .AddImports("System", "System.Linq", "System.Collections.Generic", "Discord");

        public async Task<EvalResult> EvaluateAsync(string code)
        {
            try
            {
                var result = await CSharpScript.EvaluateAsync(code, _scriptOptions);
                return new EvalResult(result.ToString(), null);
            }
            catch (CompilationErrorException ex)
            {
                return new EvalResult(null, ex.Diagnostics);
            }

        }
    }
}
