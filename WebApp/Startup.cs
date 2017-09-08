using System.Threading.Tasks;
using System.Collections.Specialized;

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

namespace AzureSkyMedia.WebApp
{
    public class Startup
    {
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
            options.ShowRequestHeaders();
            options.ShowJsonEditor();
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
                if (context.Properties.Items.ContainsKey("SubDomain"))
                {
                    string subDomain = context.Properties.Items["SubDomain"];
                    if (!string.IsNullOrEmpty(subDomain))
                    {
                        subDomain = string.Concat(char.ToUpper(subDomain[0]), subDomain.Substring(1));
                        policyId = string.Concat(policyId, subDomain);
                    }
                }
            }
            return policyId;
        }

        private static Task OnAuthenticationRedirect(RedirectContext context)
        {
            string policyId = GetPolicyId(context);
            if (!string.IsNullOrEmpty(policyId))
            {
                context.ProtocolMessage.Parameters.Add("p", policyId);
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
            CacheClient cacheClient = new CacheClient(authToken);
            string itemKey = Constant.Cache.ItemKey.MediaProcessors;
            cacheClient.SetValue<NameValueCollection>(itemKey, null);

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
            authBuilder.AddOpenIdConnect(openIdOptions =>
            {
                string settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
                openIdOptions.Authority = AppSetting.ConfigRoot[settingKey];

                settingKey = Constant.AppSettingKey.DirectoryClientId;
                openIdOptions.ClientId = AppSetting.ConfigRoot[settingKey];

                settingKey = Constant.AppSettingKey.DirectoryClientSecret;
                openIdOptions.ClientSecret = AppSetting.ConfigRoot[settingKey];

                openIdOptions.Events = new OpenIdConnectEvents
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