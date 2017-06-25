using System.Threading.Tasks;
using System.Collections.Specialized;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            configBuilder.AddJsonFile(Constant.ConfigFile, false, true);
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
            }
        }

        private Info GetApiInfo()
        {
            Info apiInfo = new Info();
            string settingKey = Constant.AppSettingKey.AppApiTitle;
            apiInfo.Title = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiDescription;
            apiInfo.Description = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiVersion;
            apiInfo.Version = AppSetting.GetValue(settingKey);
            return apiInfo;
        }

        private void SetApiOptions(SwaggerGenOptions options)
        {
            Info apiInfo = GetApiInfo();
            options.SwaggerDoc(apiInfo.Version, apiInfo);
        }

        private void SetApiOptions(SwaggerUIOptions options)
        {
            string settingKey = Constant.AppSettingKey.AppApiEndpointUrl;
            string endpointUrl = AppSetting.GetValue(settingKey);
            settingKey = Constant.AppSettingKey.AppApiVersion;
            string apiVersion = AppSetting.GetValue(settingKey);
            options.SwaggerEndpoint(endpointUrl, apiVersion);
            options.ShowRequestHeaders();
            options.ShowJsonEditor();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(AppSetting.ConfigRoot);
            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddMvc();
            services.AddSwaggerGen(SetApiOptions);
        }

        private static string GetPolicyId(RedirectContext context)
        {
            string settingKey = null;
            string policyId = string.Empty;
            string[] requestPath = context.Request.Path.Value.Split('/');
            string requestAction = requestPath[requestPath.Length - 1].ToLower();
            switch (requestAction)
            {
                case "signin":
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
                        subDomain = char.ToUpper(subDomain[0]) + subDomain.Substring(1);
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

        public static RedirectToActionResult OnSignIn(string authToken, ControllerBase controller)
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log)
        {
            IConfigurationSection loggingSection = AppSetting.ConfigRoot.GetSection("Logging");
            log.AddConsole(loggingSection);
            log.AddDebug();

            OpenIdConnectOptions openIdOptions = new OpenIdConnectOptions();
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

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            app.UseStaticFiles();
            app.UseCookieAuthentication();
            app.UseOpenIdConnectAuthentication(openIdOptions);
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(SetApiOptions);
        }
    }
}
