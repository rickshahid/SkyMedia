using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AzureSkyMedia.PlatformServices
{
    internal static class AuthToken
    {
        private static Claim GetTokenClaim(string authToken, string claimType)
        {
            Claim tokenClaim = null;
            if (!string.IsNullOrEmpty(authToken))
            {
                JwtSecurityToken securityToken = new JwtSecurityToken(authToken);
                foreach (Claim claim in securityToken.Claims)
                {
                    if (string.Equals(claim.Type, claimType, StringComparison.OrdinalIgnoreCase))
                    {
                        tokenClaim = claim;
                    }
                }
            }
            return tokenClaim;
        }

        public static string[] GetClaimValues(string authToken, string claimType)
        {
            string[] claimValues = new string[] { };
            Claim tokenClaim = GetTokenClaim(authToken, claimType);
            if (tokenClaim != null)
            {
                if (tokenClaim.Value.Contains(Constant.TextDelimiter.Application.ToString()))
                {
                    claimValues = tokenClaim.Value.Split(Constant.TextDelimiter.Application);
                }
                else
                {
                    claimValues = new string[] { tokenClaim.Value };
                }
            }
            return claimValues;
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
    }
}