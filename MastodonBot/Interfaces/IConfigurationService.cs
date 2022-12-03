using MastodonBot.Models;

namespace MastodonBot.Interfaces
{
    public interface IConfigurationService
    {
         MastodonUser? GetUser();
         string GetAppName();
         string GetInstance();
    }
}