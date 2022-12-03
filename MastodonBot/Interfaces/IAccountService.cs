namespace MastodonBot.Interfaces
{
    public interface IAccountService
    {
         Task FollowBack(
            string accountIdToFollow);
    }
}