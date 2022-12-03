using Microsoft.Extensions.Logging;
using MastodonBot.Interfaces;

namespace MastodonBot.Services
{
    public class AccountService: IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IRegistrationService _registrationService;

        public AccountService(
            ILogger<AccountService> logger,
            IRegistrationService registrationService)
        {
            _logger = logger;
            _registrationService = registrationService;
        }

        public async Task FollowBack(
            string accountIdToFollow)
        {
            var client = await _registrationService.GetMastodonClient();

            await client.Follow(
                accountIdToFollow,
                false);

            // maybe mute later on to prevent un-needed notifications
        }
    }
}