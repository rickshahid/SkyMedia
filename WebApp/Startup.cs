using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;

using AzureSkyMedia.PlatformServices;
using AzureSkyMedia.WebApp.Controllers;

namespace AzureSkyMedia.WebApp
{
    public class Startup
    {
        private static string _defaultDirectoryId;

        public Startup(IHostingEnvironment env)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(env.ContentRootPath);
            configBuilder.AddJsonFile(Constant.AppSettings, false, true);
            configBuilder.AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                configBuilder.AddUserSecrets<Startup>();
            }
            AppSetting.ConfigRoot = configBuilder.Build();
            string settingKey = Constant.AppSettingKey.AppInsightsInstrumentationKey;
            string appInsightsKey = AppSetting.GetValue(settingKey);
            if (!string.IsNullOrEmpty(appInsightsKey))
            {
                configBuilder.AddApplicationInsightsSettings(instrumentationKey: appInsightsKey);
                AppSetting.ConfigRoot = configBuilder.Build();
            }
        }

        private void SetSwaggerOptions(SwaggerGenOptions options)
        {
            Info apiInfo = new Info();
            string settingKey = Constant.AppSettingKey.AppApiTitle;
            apiInfo.Title = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiDescription;
            apiInfo.Description = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiVersion;
            apiInfo.Version = AppSetting.GetValue(settingKey);
            options.SwaggerDoc(apiInfo.Version, apiInfo);
        }

        private void SetSwaggerOptions(SwaggerUIOptions options)
        {
            string settingKey = Constant.AppSettingKey.AppApiEndpointUrl;
            string endpointUrl = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiVersion;
            string apiVersion = AppSetting.GetValue(settingKey);
            options.SwaggerEndpoint(endpointUrl, apiVersion);
        }

        private static string GetPolicyId(RedirectContext context)
        {
            string policyId = string.Empty;
            string settingKey = string.Empty;
            string[] requestPath = context.Request.Path.Value.Split('/');
            string requestAction = requestPath[requestPath.Length - 1].ToLower();
            switch (requestAction)
            {
                case "signupin":
                    settingKey = Constant.AppSettingKey.DirectoryPolicyIdSignUpIn;
                    break;
                case "profileedit":
                    settingKey = Constant.AppSettingKey.DirectoryPolicyIdProfileEdit;
                    break;
                case "passwordreset":
                    settingKey = Constant.AppSettingKey.DirectoryPolicyIdPasswordReset;
                    break;
            }
            if (!string.IsNullOrEmpty(settingKey))
            {
                policyId = AppSetting.GetValue(settingKey);
            }
            return policyId;
        }

        private static void SetOpenIdOptions(OpenIdConnectOptions options, string directoryId, string directoryTenantId)
        {
            options.Authority = AuthToken.GetIssuerUrl(directoryId, directoryTenantId);
            options.MetadataAddress = AuthToken.GetDiscoveryUrl(directoryId, directoryTenantId);

            string settingKey = Constant.AppSettingKey.DirectoryClientId;
            settingKey = string.Format(settingKey, directoryId);
            options.ClientId = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.DirectoryClientSecret;
            settingKey = string.Format(settingKey, directoryId);
            options.ClientSecret = AppSetting.GetValue(settingKey);
        }

        private static Task OnAuthenticationRedirect(RedirectContext context)
        {
            string directoryId = homeController.GetDirectoryId(context.Request);
            if (!string.Equals(directoryId, _defaultDirectoryId, StringComparison.OrdinalIgnoreCase))
            {
                string settingKey = Constant.AppSettingKey.DirectoryTenantId;
                settingKey = string.Format(settingKey, directoryId);
                string directoryTenantId = AppSetting.GetValue(settingKey);

                SetOpenIdOptions(context.Options, directoryId, directoryTenantId);

                context.ProtocolMessage.IssuerAddress = AuthToken.GetAuthorizeUrl(directoryId, directoryTenantId);
                context.ProtocolMessage.ClientId = context.Options.ClientId;
            }

            if (string.Equals(directoryId, Constant.DirectoryService.B2C, StringComparison.OrdinalIgnoreCase))
            {
                string policyId = GetPolicyId(context);
                if (!string.IsNullOrEmpty(policyId))
                {
                    context.ProtocolMessage.Parameters.Add("p", policyId);
                }
            }

            context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("signin-oidc", string.Empty);
            if (!context.ProtocolMessage.RedirectUri.Contains("localhost"))
            {
                context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http:", "https:");
            }

            return Task.FromResult(0);
        }

        internal static RedirectToActionResult OnSignIn(ControllerBase controller, string authToken)
        {
            RedirectToActionResult redirectAction = null;
            string requestError = controller.Request.Form["error_description"];
            if (!string.IsNullOrEmpty(requestError) && requestError.Contains(Constant.Message.UserPasswordForgotten))
            {
                redirectAction = controller.RedirectToAction("passwordreset", "account");
            }
            return redirectAction;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(AppSetting.ConfigRoot);
            AuthenticationBuilder authBuilder = services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });
            authBuilder.AddOpenIdConnect(options =>
            {
                _defaultDirectoryId = homeController.GetDirectoryId(null);

                string settingKey = Constant.AppSettingKey.DirectoryTenantId;
                settingKey = string.Format(settingKey, _defaultDirectoryId);
                string directoryTenantId = AppSetting.GetValue(settingKey);

                SetOpenIdOptions(options, _defaultDirectoryId, directoryTenantId);

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = OnAuthenticationRedirect
                };
            });
            services.AddMvc();
            services.AddSwaggerGen(SetSwaggerOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log)
        {
            IConfigurationSection loggingSection = AppSetting.ConfigRoot.GetSection("Logging");
            log.AddConsole(loggingSection);
            log.AddDebug();
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(SetSwaggerOptions);
        }
    }
}