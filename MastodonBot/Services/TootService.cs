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
            stream.OnUpdate += OnUpdate;
            stream.OnNotification += OnNotificataion;
        }

        private void UnregisterDelegates(TimelineStreaming stream)
        {
            stream.OnUpdate -= OnUpdate;
            stream.OnNotification -= OnNotificataion;
        }

        private void OnUpdate(object? sender, StreamUpdateEventArgs e)
        {
            _logger.LogInformation(e.Status.Content);
        }

        private async void OnNotificataion(object? sender, StreamNotificationEventArgs e)
        {
            try
            {
                if (e.Notification.Type == "mention")
                {
                    var message = $"Got message from {e.Notification.Account.AccountName}: {e.Notification.Status.Content}";
                    _logger.LogInformation(message);

                    var responseStatusId = await _responseService.RespondWithRandomMessage(
                        e.Notification.Account.AccountName,
                        e.Notification.Account.DisplayName,
                        e.Notification.Status.Id);

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