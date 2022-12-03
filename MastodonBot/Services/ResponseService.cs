using Microsoft.Extensions.Logging;
using MastodonBot.Interfaces;
using MastodonBot.Enums;
using System.Text.RegularExpressions;
using MastodonBot.Utilities;

namespace MastodonBot.Services
{
    public class ResponseService: IResponseService
    {
        private const string StripHtmlPattern = "<.*?>";

        private readonly ILogger<ResponseService> _logger;
        private readonly IRegistrationService _registrationService;

        public ResponseService(
            ILogger<ResponseService> logger,
            IRegistrationService registrationService)
        {
            _logger = logger;
            _registrationService = registrationService;
        }

        public async Task ParseAndReply(
            string originalMessage,
            string replyToAccountName,
            string replyToDisplayName,
            string originalStatusId)
        {
            var responseTypes = ParseMessage(originalMessage);

            var client = await _registrationService.GetMastodonClient();

            foreach (var responseType in responseTypes)
            {
                var responseMsessage = GetResponseStatusMessage(
                    responseType,
                    originalMessage,
                    replyToAccountName, 
                    replyToDisplayName);

                var resultStatus = await client.PublishStatus(
                    responseMsessage,
                    replyStatusId: originalStatusId);

                _logger.LogInformation($"Responded to {replyToAccountName} ({originalStatusId}) with {resultStatus.Id}");
            }
        }

        private List<ResponseType> ParseMessage(
            string message)
        {
            message = StripHtml(message);

            var result = new List<ResponseType>();

            foreach (var pattern in ResponseTypeUtility.ResponseTypePatterns)
            {
                if (Regex.Match(message, pattern.Pattern)?.Success ?? false)
                {
                    result.Add(pattern.ResponseType);
                }
            }

            if (!result.Any())
            {
                result.Add(ResponseType.Unknown);
            }

            return result;
        }

        /// yes this is not optimal
        private string StripHtml(string message) => Regex.Replace(message, StripHtmlPattern, string.Empty);


        private string GetResponseStatusMessage(
            ResponseType responseType,
            string originalMessage,
            string replyToAccountName,
            string replyToDisplayName)
        {
            switch (responseType)
            {
                case ResponseType.CoinFlip:
                    return GetCoinFlipResponse();
                case ResponseType.DiceRoll:
                    return GetDiceRollResponse();
                case ResponseType.Unknown:
                default:
                    return GetUnknownResponse();
            }
        }

        private string GetCoinFlipResponse()
        {
            return string.Empty;
        }

        private string GetDiceRollResponse()
        {
            return string.Empty;
        }

        private string GetUnknownResponse()
        {
            return string.Empty;
        }
    }
}