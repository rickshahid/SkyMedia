using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AzureSkyMedia.PlatformServices
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
                if (tokenClaim.Value.Contains(Constant.TextDelimiter.Application.ToString()))
                {
                    claimValues = tokenClaim.Value.Split(Constant.TextDelimiter.Application);
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
    }
}
