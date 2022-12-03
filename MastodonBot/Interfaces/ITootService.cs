namespace MastodonBot.Interfaces
{
    public interface ITootService
    {
         Task Execute(CancellationToken cancellationToken);
    }
}