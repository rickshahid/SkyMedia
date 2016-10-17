using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;

namespace SkyMedia.ServiceBroker
{
    public static class AuthToken
    {
        private static Claim GetTokenClaim(string authToken, string claimName)
        {
            Claim tokenClaim = null;
            if (!string.IsNullOrEmpty(authToken))
            {
                JwtSecurityToken token = new JwtSecurityToken(authToken);
                foreach (Claim claim in token.Claims)
                {
                    if (string.Equals(claim.Type, claimName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tokenClaim = claim;
                    }
                }
            }
            return tokenClaim;
        }

        public static string GetClaimValue(string authToken, string claimName)
        {
            string claimValue = string.Empty;
            Claim tokenClaim = GetTokenClaim(authToken, claimName);
            if (tokenClaim != null)
            {
                claimValue = tokenClaim.Value.Trim();
            }
            return claimValue;
        }

        public static string[] GetClaimValues(string authToken, string claimName)
        {
            string[] claimValues = new string[] { };
            Claim tokenClaim = GetTokenClaim(authToken, claimName);
            if (tokenClaim != null)
            {
                if (tokenClaim.Value.Contains(Constants.MultiItemSeparator.ToString()))
                {
                    claimValues = tokenClaim.Value.Split(Constants.MultiItemSeparator);
                    for (int i = 0; i < claimValues.Length; i++)
                    {
                        claimValues[i] = claimValues[i].Trim();
                    }
                }
                else
                {
                    claimValues = new string[] { tokenClaim.Value.Trim() };
                }
            }
            return claimValues;
        }

        public static string GetValue(HttpRequest request, HttpResponse response)
        {
            string authToken = null;
            string cookieKey = Constants.HttpCookies.UserAuthToken;
            if (request.HasFormContentType)
            {
                authToken = request.Form[Constants.HttpForm.IdToken];
                if (!string.IsNullOrEmpty(authToken))
                {
                    response.Cookies.Append(cookieKey, authToken);
                }
            }
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = request.Cookies[cookieKey];
            }
            return authToken;
        }
    }
}
