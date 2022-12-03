namespace MastodonBot.Models
{
    public class MastodonUser
    {
        public const string ConfigSectionName = "mastodonUser";

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}