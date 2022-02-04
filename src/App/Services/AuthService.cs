using App.Services.Interfaces;
using System.Threading.Tasks;
using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Shared;

namespace App.Services
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _poolId;
        private readonly string _clientId;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _poolId = _configuration.GetValue<string>(Constants.SSMCognitoUserPoolId);
            _clientId = _configuration.GetValue<string>(Constants.SSMCognitoClientId);
        }

        public async Task<string> SignIn(string username, string password)
        {
            CognitoUserPool userPool = new CognitoUserPool(_poolId, _clientId, provider);
            CognitoUser user = new CognitoUser(username, _clientId, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var accessToken = authResponse.AuthenticationResult.AccessToken;

            return accessToken;
        }
    }
}
