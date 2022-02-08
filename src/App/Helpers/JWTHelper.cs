using Amazon.Lambda.APIGatewayEvents;
using App.Models;
using System.IdentityModel.Tokens.Jwt;

namespace App.Helpers
{
    public class JWTHelper
    {
        public TokenData GetTokenData(APIGatewayProxyRequest request)
        {
            var data = new TokenData();
            var token = Decode(GetTokenFromAPIGatewayProxyRequest(request));

            if (token != null)
            {
                data.Username = token.Payload["username"].ToString();
                data.UserId = new System.Guid(token.Payload["sub"].ToString());
            }

            return data;
        }

        private string GetTokenFromAPIGatewayProxyRequest (APIGatewayProxyRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
                return null;

            var token = request.Headers["Authorization"];
            if (token.ToLower().StartsWith("bearer"))
                token = token.Substring("bearer".Length).Trim();

            return token;
        }

        private JwtSecurityToken Decode(string token)
        {
            if (token == null) return null;
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }        
    }
}
