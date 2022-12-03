namespace MastodonBot.Interfaces
{
    public interface IResponseService
    {
         Task ParseAndReply(
            string originalMessage,
            string replyToAccountName,
            string replyToDisplayName,
            string originalStatusId);
    }
}