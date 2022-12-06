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
                var matches = Regex.Matches(message, pattern.Pattern, RegexOptions.IgnoreCase);

                if (matches.Any())
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
                    return GetCoinFlipResponse(
                        replyToAccountName,
                        replyToDisplayName);
                case ResponseType.DiceRoll:
                    return GetDiceRollResponse(
                        replyToAccountName,
                        replyToDisplayName);
                case ResponseType.Unknown:
                default:
                    return GetUnknownResponse(
                        replyToAccountName);
            }
        }

        private string GetCoinFlipResponse(
            string replyToAccountName,
            string replyToDisplayName)
        {
            var random = new Random();

            var flipResult = random.Next(0, 1) == 0
                ? "heads"
                : "tails";

            var result = $"@{replyToAccountName} Hey {replyToDisplayName} you flipped a {flipResult}!";

            return result;
        }

        private string GetDiceRollResponse(
            string replyToAccountName,
            string replyToDisplayName)
        {
            var random = new Random();

            var rollResult = random.Next(1, 6);

            var result = $"@{replyToAccountName} Hey {replyToDisplayName} you rolled a {rollResult}!";

            return result;
        }

        private string GetUnknownResponse(
            string replyToAccountName)
        {
            var result = $"@{replyToAccountName} Sorry, I either couldn't figure out what you asked for, or you asked for a feature that I do not support.";

            return result;
        }
    }
}