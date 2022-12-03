namespace MastodonBot.Interfaces
{
    public interface IResponseService
    {
         Task<string> RespondWithRandomMessage(
            string replyToAccountName,
            string replyToDisplayName,
            string originalStatusId);
    }
}