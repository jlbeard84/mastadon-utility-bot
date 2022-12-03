using System.Text.Json;
using Mastonet;
using Mastonet.Entities;
using Microsoft.Extensions.Logging;
using MastodonBot.Interfaces;

namespace MastodonBot.Services
{
    public class RegistrationService: IRegistrationService
    {
        private const string RegistrationFileName = "registration.json";

        private readonly ILogger<RegistrationService> _logger;
        private readonly IConfigurationService _configService;

        private AuthenticationClient? _authClient = null;
        private MastodonClient? _client = null;
        private AppRegistration? _appRegistration;
        private Auth? _auth = null;

        public RegistrationService(
            ILogger<RegistrationService> logger,
            IConfigurationService configService)
        {
            _logger = logger;
            _configService = configService;
        }

        public async Task Register()
        { 
            if (_appRegistration != null)
            {
                // app is already registered
                return;
            }

            var existingRegistration = await ReadRegistrationFile();

            if (existingRegistration == null)
            {
                var authClient = GetAuthClient();
                var appName = _configService.GetAppName();

                var appRegistration = await authClient.CreateApp(
                    appName,
                    Scope.Read | Scope.Write | Scope.Follow);

                _appRegistration = appRegistration;

                await WriteRegistrationFile(appRegistration);

                _logger.LogInformation($"App registered: {appRegistration.ClientId}");
            }
            else
            {
                _appRegistration = existingRegistration;
                _logger.LogInformation($"App already registered: {existingRegistration.ClientId}");
            }
        }

        public AuthenticationClient GetAuthClient()
        {
            if (_authClient == null)
            {
                if (_appRegistration != null)
                {
                    _authClient = new AuthenticationClient(_appRegistration);
                }
                else
                {
                    var instance = _configService.GetInstance() ?? string.Empty;
                    _authClient = new AuthenticationClient(instance);
                }
            }

            return _authClient;
        }

        public async Task<MastodonClient> GetMastodonClient()
        {
            if (_client == null)
            {
                if (_appRegistration == null)
                {
                    throw new Exception("App is not registered");
                }

                if (_auth == null)
                {
                    var user = _configService.GetUser();

                    if (user == null)
                    {
                        throw new Exception("User is null");
                    }

                    var authClient = GetAuthClient();

                    var auth = await authClient.ConnectWithPassword(
                        user.Email,
                        user.Password);

                    _auth = auth;
                }

                var client = new MastodonClient(
                    _configService.GetInstance(),
                    _auth.AccessToken);

                _client = client;

                _logger.LogInformation($"Logged in with token created at {_auth.CreatedAt}");
            }

            return _client;
        }

        private async Task<AppRegistration?> ReadRegistrationFile()
        {
            AppRegistration? registration = null;

            try
            {
                var serilizedReg = await File.ReadAllTextAsync(RegistrationFileName);
                registration = JsonSerializer.Deserialize<AppRegistration>(serilizedReg);
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    ex, 
                    $"Error in {nameof(RegistrationService)}.{nameof(RegistrationService.ReadRegistrationFile)}");
            }
            
            return registration;
        }

        private async Task WriteRegistrationFile(AppRegistration registration)
        {
            var serializedReg = JsonSerializer.Serialize(registration);
            await File.WriteAllTextAsync(RegistrationFileName, serializedReg);
        }
    }
}