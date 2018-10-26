using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Rest;
using Microsoft.Identity.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal static class AuthToken
    {
        private static Claim GetTokenClaim(string authToken, string claimType)
        {
            Claim tokenClaim = null;
            JwtSecurityToken securityToken = new JwtSecurityToken(authToken);
            foreach (Claim claim in securityToken.Claims)
            {
                if (string.Equals(claim.Type, claimType, StringComparison.OrdinalIgnoreCase))
                {
                    tokenClaim = claim;
                }
            }
            return tokenClaim;
        }

        public static string GetClaimValue(string authToken, string claimType)
        {
            string claimValue = string.Empty;
            Claim tokenClaim = GetTokenClaim(authToken, claimType);
            if (tokenClaim != null)
            {
                claimValue = tokenClaim.Value;
            }
            return claimValue;
        }

        public static TokenCredentials AcquireToken(string authToken, out string subscriptionId)
        {
            User userProfile = new User(authToken);
            subscriptionId = userProfile.MediaAccount.SubscriptionId;

            AuthenticationResult authResult = userProfile.MediaAccount.AcquireToken().Result;
            return new TokenCredentials(authResult.AccessToken);
        }
    }
}