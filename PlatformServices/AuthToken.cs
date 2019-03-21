using System;
using System.Threading.Tasks;
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

        public static TokenCredentials AcquireToken(MediaAccount mediaAccount)
        {
            AuthenticationResult authResult = AcquireTokenAsync(mediaAccount).Result;
            return new TokenCredentials(authResult.AccessToken);
        }

        public static Task<AuthenticationResult> AcquireTokenAsync(MediaAccount mediaAccount)
        {
            string settingKey = Constant.AppSettingKey.DirectoryAuthorityUrl;
            string authorityUrl = AppSetting.GetValue(settingKey);
            authorityUrl = string.Format(authorityUrl, mediaAccount.DirectoryTenantId);

            string redirectUri = Constant.AuthIntegration.OpenAuthRedirectUri;
            ClientCredential clientCredential = new ClientCredential(mediaAccount.ServicePrincipalKey);
            ConfidentialClientApplication clientApplication = new ConfidentialClientApplication(mediaAccount.ServicePrincipalId, authorityUrl, redirectUri, clientCredential, null, null);

            settingKey = Constant.AppSettingKey.DirectoryTokenScope;
            string tokenScope = AppSetting.GetValue(settingKey);

            string[] tokenScopes = new string[] { tokenScope };
            return clientApplication.AcquireTokenForClientAsync(tokenScopes);
        }
    }
}