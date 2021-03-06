﻿using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment hosting)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(hosting.ContentRootPath);
            configBuilder.AddEnvironmentVariables();
            configBuilder.AddApplicationInsightsSettings();
            configBuilder.AddJsonFile(Constant.AppSettingsFile, false, true);
            if (Debugger.IsAttached)
            {
                configBuilder.AddUserSecrets<Startup>();
            }
            AppSetting.Configuration = configBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(AppSetting.Configuration);
            AuthenticationBuilder authBuilder = services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });
            authBuilder.AddOpenIdConnect(options =>
            {
                string settingKey = Constant.AppSettingKey.DirectoryTenantId;
                string tenantId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryAuthorityUrl;
                string authorityUrl = AppSetting.GetValue(settingKey);
                authorityUrl = string.Format(authorityUrl, tenantId);

                settingKey = Constant.AppSettingKey.DirectoryDiscoveryPath;
                string discoveryPath = AppSetting.GetValue(settingKey);

                options.Authority = authorityUrl;
                options.MetadataAddress = string.Concat(authorityUrl, discoveryPath);

                settingKey = Constant.AppSettingKey.DirectoryClientId;
                options.ClientId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryClientSecret;
                options.ClientSecret = AppSetting.GetValue(settingKey);

                options.CallbackPath = "/";
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = OnAuthenticationRedirect
                };
            });
            services.AddMvc();
            services.AddSwaggerGen(SetSwaggerOptions);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(SetSwaggerOptions);
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        internal static RedirectToActionResult OnSignIn(ControllerBase controller)
        {
            RedirectToActionResult redirectAction = null;
            string requestError = controller.Request.Form["error_description"];
            if (!string.IsNullOrEmpty(requestError) && requestError.Contains(Constant.Message.UserPasswordForgotten))
            {
                redirectAction = controller.RedirectToAction("passwordReset", "account");
            }
            return redirectAction;
        }

        private static Task OnAuthenticationRedirect(RedirectContext context)
        {
            string policyId = GetPolicyId(context);
            if (!string.IsNullOrEmpty(policyId))
            {
                context.ProtocolMessage.Parameters.Add("p", policyId);
            }
            if (!context.ProtocolMessage.RedirectUri.Contains("localhost"))
            {
                context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http:", "https:");
            }
            return Task.FromResult(0);
        }

        private static string GetPolicyId(RedirectContext context)
        {
            string policyId = string.Empty;
            string settingKey = string.Empty;
            string[] requestPath = context.Request.Path.Value.Split('/');
            string requestAction = requestPath[requestPath.Length - 1];
            switch (requestAction)
            {
                case "signUpIn":
                    settingKey = Constant.AppSettingKey.DirectoryPolicyIdSignUpIn;
                    break;
                case "profileEdit":
                    settingKey = Constant.AppSettingKey.DirectoryPolicyIdProfileEdit;
                    break;
                case "passwordReset":
                    settingKey = Constant.AppSettingKey.DirectoryPolicyIdPasswordReset;
                    break;
            }
            if (!string.IsNullOrEmpty(settingKey))
            {
                policyId = AppSetting.GetValue(settingKey);
            }
            return policyId;
        }

        private void SetSwaggerOptions(SwaggerGenOptions options)
        {
            string settingKey = Constant.AppSettingKey.AppName;
            string appName = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiDescription;
            string apiDescription = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiVersion;
            string apiVersion = AppSetting.GetValue(settingKey);
            Info apiInfo = new Info()
            {
                Title = string.Concat(appName, " API"),
                Description = apiDescription,
                Version = apiVersion
            };
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
    }
}