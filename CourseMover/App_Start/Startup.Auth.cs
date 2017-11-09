using System;
using Microsoft.Owin.Security.Cookies;
using Owin;
using CourseMover.Web.Providers.Canvas;
using Microsoft.Owin.Security;
using System.Configuration;

namespace CourseMover
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Set auth to use external cookie
            app.SetDefaultSignInAsAuthenticationType("ExternalCookie");
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = ".AspNet.ExternalCookie",
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
            });

            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            var canvasOptions = new CanvasAuthenticationOptions()
            {
                Endpoints =
                    new CanvasAuthenticationOptions.CanvasAuthenticationEndpoints()
                    {
                        AuthorizationEndpoint = baseUrl + "login/oauth2/auth",
                        TokenEndpoint = baseUrl + "login/oauth2/token"
                    },
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                ClientKey = ConfigurationManager.AppSettings["ClientKey"]
            };

            app.UseCanvasAuthentication(canvasOptions);
        }
    }
}