using System.Threading.Tasks;

namespace DiscordBotV5.Services.Eval
{
    public interface IEvalService
    {
        Task<EvalResult> EvaluateAsync(string code);
    }
}
