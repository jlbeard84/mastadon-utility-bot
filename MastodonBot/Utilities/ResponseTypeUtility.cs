using MastodonBot.Enums;
using MastodonBot.Models;

namespace MastodonBot.Utilities
{
    public static class ResponseTypeUtility
    {
        private const string DiceRollPattern = "(\\broll\\b.*\\b(dice|die)\\b|\\b(dice|die)\\b.*\\broll\\b)";
        private const string CoinFlipPattern = "(\\bflip\\b.*\\bcoin\\b|\\bcoin\b.*\\bflip\\b)";

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