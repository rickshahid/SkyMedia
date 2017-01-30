using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Swashbuckle.Swagger.Model;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(env.ContentRootPath);
            configBuilder.AddJsonFile("appsettings.json", false, true);
            configBuilder.AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                configBuilder.AddUserSecrets();
            }
            AppSetting.ConfigRoot = configBuilder.Build();
            string settingKey = Constants.AppSettingKey.AppInsightsInstrumentationKey;
            string appInsightsKey = AppSetting.GetValue(settingKey);
            if (!string.IsNullOrEmpty(appInsightsKey))
            {
                configBuilder.AddApplicationInsightsSettings(instrumentationKey: appInsightsKey);
                AppSetting.ConfigRoot = configBuilder.Build();
            }
        }

        private Info GetApiInfo()
        {
            Info apiInfo = new Info();
            string settingKey = Constants.AppSettingKey.AppApiTitle;
            apiInfo.Title = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.AppApiDescription;
            apiInfo.Description = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.AppApiVersion;
            apiInfo.Version = AppSetting.GetValue(settingKey);
            return apiInfo;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(AppSetting.ConfigRoot);
            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddMvc();
            services.AddSwaggerGen();
            Info apiInfo = GetApiInfo();
            services.ConfigureSwaggerGen(options => options.SingleApiVersion(apiInfo));
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
                    settingKey = Constants.AppSettingKey.DirectoryPolicyIdSignUpIn;
                    break;
                case "profileedit":
                    settingKey = Constants.AppSettingKey.DirectoryPolicyIdProfileEdit;
                    break;
                case "passwordreset":
                    settingKey = Constants.AppSettingKey.DirectoryPolicyIdPasswordReset;
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log)
        {
            IConfigurationSection loggingSection = AppSetting.ConfigRoot.GetSection("Logging");
            log.AddConsole(loggingSection);
            log.AddDebug();

            OpenIdConnectOptions openIdOptions = new OpenIdConnectOptions();
            string settingKey = Constants.AppSettingKey.DirectoryIssuerUrl;
            openIdOptions.Authority = AppSetting.ConfigRoot[settingKey];

            settingKey = Constants.AppSettingKey.DirectoryClientId;
            openIdOptions.ClientId = AppSetting.ConfigRoot[settingKey];

            settingKey = Constants.AppSettingKey.DirectoryClientSecret;
            openIdOptions.ClientSecret = AppSetting.ConfigRoot[settingKey];

            openIdOptions.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = OnAuthenticationRedirect
            };

            app.UseApplicationInsightsRequestTelemetry();
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseApplicationInsightsExceptionTelemetry();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            app.UseStaticFiles();
            app.UseCookieAuthentication();
            app.UseOpenIdConnectAuthentication(openIdOptions);
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
