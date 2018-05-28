﻿using System;
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

        public static string GetIssuerUrl(string directoryId, string directoryTenant)
        {
            string settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
            string issuerUrl = AppSetting.GetValue(settingKey);
            issuerUrl = string.Format(issuerUrl, directoryTenant);
            if (string.Equals(directoryId, Constant.DirectoryService.B2B, StringComparison.OrdinalIgnoreCase))
            {
                issuerUrl = issuerUrl.Replace("/v2.0", string.Empty);
            }
            return issuerUrl;
        }

        public static string GetDiscoveryUrl(string directoryId, string directoryTenantId)
        {
            string settingKey = Constant.AppSettingKey.DirectoryDiscoveryUrl;
            string discoveryUrl = AppSetting.GetValue(settingKey);
            discoveryUrl = string.Format(discoveryUrl, directoryTenantId);
            if (string.Equals(directoryId, Constant.DirectoryService.B2B, StringComparison.OrdinalIgnoreCase))
            {
                discoveryUrl = discoveryUrl.Replace("/v2.0", string.Empty);
            }
            return discoveryUrl;
        }

        public static string GetAuthorizeUrl(string directoryId, string directoryTenantId)
        {
            string issuerUrl = GetIssuerUrl(directoryId, directoryTenantId);
            issuerUrl = issuerUrl.Replace("/v2.0", string.Empty);
            string authorizeUrl = string.Concat(issuerUrl, "oauth2/v2.0/authorize");
            if (string.Equals(directoryId, Constant.DirectoryService.B2B, StringComparison.OrdinalIgnoreCase))
            {
                authorizeUrl = authorizeUrl.Replace("/v2.0", string.Empty);
            }
            return authorizeUrl;
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