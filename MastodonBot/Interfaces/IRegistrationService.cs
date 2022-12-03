using Mastonet;

namespace MastodonBot.Interfaces
{
    public interface IRegistrationService
    {
        Task Register();
        AuthenticationClient GetAuthClient();
        Task<MastodonClient> GetMastodonClient();
    }
}