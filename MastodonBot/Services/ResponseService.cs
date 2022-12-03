using Microsoft.Extensions.Logging;
using MastodonBot.Interfaces;

namespace MastodonBot.Services
{
    public class ResponseService: IResponseService
    {
        private readonly ILogger<ResponseService> _logger;
        private readonly IRegistrationService _registrationService;

        public ResponseService(
            ILogger<ResponseService> logger,
            IRegistrationService registrationService)
        {
            _logger = logger;
            _registrationService = registrationService;
        }

        public async Task<string> RespondWithRandomMessage(
            string replyToAccountName,
            string replyToDisplayName,
            string originalStatusId)
        {
            var client = await _registrationService.GetMastodonClient();

            var responseMsessage = GetResponseStatusMessage(
                replyToAccountName, 
                replyToDisplayName);

            var resultStatus = await client.PublishStatus(
                responseMsessage,
                replyStatusId: originalStatusId);

            _logger.LogInformation($"Responded to {replyToAccountName} ({originalStatusId}) with {resultStatus.Id}");

            return resultStatus.Id;
        }

        private string GetResponseStatusMessage(
            string replyToAccountName,
            string replyToDisplayName)
        {
            var random = new Random();
            var instance = random.Next(1, 5);

            var responseMessage = string.Empty;

            switch(instance)
            {
                case 1:
                    responseMessage = $"@{replyToAccountName} Hey {replyToDisplayName}, thanks for the message!";
                    break;
                case 2:
                    responseMessage = $"Yo dawg @{replyToAccountName}!!";
                    break;
                case 3:
                    responseMessage = $"@{replyToAccountName} This is a random response to {replyToDisplayName} from a bot bleep borp.";
                    break;
                case 4:
                   responseMessage = $"@{replyToAccountName}, maybe one day someone will make me do something more interesting than respond with a random message...";
                    break;
                case 5:
                    responseMessage = $"@{replyToAccountName} I know I'm just a proof of concept but at least it works?";
                    break;
            }

            return responseMessage;
        }
    }
}