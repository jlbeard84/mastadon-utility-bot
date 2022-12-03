using Mastonet;
using Microsoft.Extensions.Logging;
using MastodonBot.Interfaces;

namespace MastodonBot.Services
{
    public class TootService: ITootService
    {
        private readonly ILogger<TootService> _logger;
        private readonly IConfigurationService _configService;
        private readonly IRegistrationService _registrationService;
        private readonly IResponseService _responseService;
        private readonly IAccountService _accountService;

        public TootService(
            ILogger<TootService> logger,
            IConfigurationService configService,
            IRegistrationService registrationService,
            IResponseService responseService,
            IAccountService accountService)
        {
            _logger = logger;
            _configService = configService;
            _registrationService = registrationService;
            _responseService = responseService;
            _accountService = accountService;
        }

        public async Task Execute(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TimelineStreaming? stream = null;

            try
            {
                await _registrationService.Register();

                var client = await _registrationService.GetMastodonClient();
                
                stream = client.GetUserStreaming();
                RegisterDelegates(stream);

                await stream.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    // loop forever for now
                    await Task.Delay(500);
                }             
            }
            catch(OperationCanceledException cancelledException)
            {
                _logger.LogInformation($"Cancellation requested in {nameof(TootService)}.{nameof(Execute)}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(TootService)}.{nameof(Execute)}");
            }
            finally
            {
                if (stream != null)
                {
                    stream.Stop();
                    UnregisterDelegates(stream);   
                }
            }
        }

        private void RegisterDelegates(TimelineStreaming stream)
        {
            stream.OnNotification += OnNotificataion;
        }

        private void UnregisterDelegates(TimelineStreaming stream)
        {
            stream.OnNotification -= OnNotificataion;
        }

        private async void OnNotificataion(object? sender, StreamNotificationEventArgs e)
        {
            try
            {
                var logMessage = $"Got incoming {e.Notification.Type} notification from e.Notification.Account.AccountName";
                _logger.LogInformation(logMessage);

                if (e.Notification.Type == "follow")
                {
                    await _accountService.FollowBack(e.Notification.Account.Id);
                }
                else if (e.Notification.Type == "mention")
                {
                    await _responseService.ParseAndReply(
                        e.Notification.Status?.Content ?? string.Empty,
                        e.Notification.Account.AccountName,
                        e.Notification.Account.DisplayName,
                        e.Notification.Status?.Id ?? string.Empty);

                    await _accountService.FollowBack(e.Notification.Account.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}