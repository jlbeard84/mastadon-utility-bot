using MastodonBot.Enums;
using MastodonBot.Models;

namespace MastodonBot.Utilities
{
    public static class ResponseTypeUtility
    {
        private const string DiceRollPattern = "^(?=.*roll)+\b(.*?)(?=.*die|dice)+\b(.*?)$";
        private const string CoinFlipPattern = "^(?=.*flip)+\b(.*?)(?=.*coin)+\b(.*?)$";

        public static readonly IReadOnlyCollection<ResponseTypePattern> ResponseTypePatterns = new List<ResponseTypePattern>
        {
            new ResponseTypePattern
            {
                Pattern = DiceRollPattern,
                ResponseType = ResponseType.DiceRoll
            },
            new ResponseTypePattern
            {
                Pattern = CoinFlipPattern,
                ResponseType = ResponseType.CoinFlip
            }
        }.AsReadOnly();
    }
}