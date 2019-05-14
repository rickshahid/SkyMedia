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
            JwtSecurityToken securityToken = new JwtSecurityToken(authToken);
            foreach (Claim claim in securityToken.Claims)
            {
                if (claim.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
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
                string[] claimValues = tokenClaim.Value.Split(Constant.TextDelimiter.Application);
                claimValue = claimValues[0].Trim();
            }
            return claimValue;
        }

        public static string[] GetClaimValues(string authToken, string claimType)
        {
            string[] claimValues = null;
            Claim tokenClaim = GetTokenClaim(authToken, claimType);
            if (tokenClaim != null)
            {
                claimValues = tokenClaim.Value.Split(Constant.TextDelimiter.Application);
                for (int i = 0; i < claimValues.Length; i++)
                {
                    claimValues[i] = claimValues[i].Trim();
                }
            }
            return claimValues;
        }
    }
}