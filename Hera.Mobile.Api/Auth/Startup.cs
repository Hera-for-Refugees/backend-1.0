using System;
using System.Threading.Tasks;
using System.Web.Http;
using Hera.Mobile.Api.Auth.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Hera.Mobile.Api.Auth.Startup))]

namespace Hera.Mobile.Api.Auth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            ConfigureOAuth(app);
            WebApiConfig.Register(httpConfig);
            app.UseWebApi(httpConfig);
        }

        void ConfigureOAuth(IAppBuilder appBuilder)
        {

            OAuthAuthorizationServerOptions oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(3),
                AllowInsecureHttp = true,
                Provider = new MobileClientAuthorizationServerProvider(),
                RefreshTokenProvider = new RefreshTokenProvider()
            };
            appBuilder.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);
            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());


        }
    }
}
