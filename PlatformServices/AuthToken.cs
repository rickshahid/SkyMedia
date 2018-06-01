using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Rest;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

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
            User authUser = new User(authToken);
            subscriptionId = authUser.MediaAccount.SubscriptionId;

            string settingKey = Constant.AppSettingKey.AzureResourceManagementAudienceUrl;
            string audienceUrl = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
            string authorityUrl = AppSetting.GetValue(settingKey);
            authorityUrl = string.Format(authorityUrl, authUser.MediaAccount.DirectoryTenantId);

            AuthenticationContext authContext = new AuthenticationContext(authorityUrl);
            ClientCredential clientCredential = new ClientCredential(authUser.MediaAccount.ClientApplicationId, authUser.MediaAccount.ClientApplicationKey);
            AuthenticationResult tokenAuth = authContext.AcquireTokenAsync(audienceUrl, clientCredential).Result;
            return new TokenCredentials(tokenAuth.AccessToken);
        }
    }
}