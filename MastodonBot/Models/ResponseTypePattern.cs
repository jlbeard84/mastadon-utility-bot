using MastodonBot.Enums;

namespace MastodonBot.Models
{
    public class ResponseTypePattern
    {
        public string Pattern {get; set; } = string.Empty;

        public ResponseType ResponseType { get; set; } = ResponseType.Unknown;
    }
}