using System.Configuration;
using Exiger.JWT.Authentication.Core;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Exiger.JWT.Core.Data.EF;
using Exiger.JWT.Core.Data.EF.Identity;

[assembly: OwinStartup(typeof(Exiger.JWT.Authentication.Startup))]
namespace Exiger.JWT.Authentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Issuer – a unique identifier for the entity that issued the token (not to be confused with Entity Framework’s entities)
            //Secret – a secret key used to secure the token and prevent tampering
            var issuer = ConfigurationManager.AppSettings["issuer"];
            var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

            //Important note: The life-cycle of object instance is per-request. As soon as the request is complete, the instance is cleaned up.
            var dbContext = (UnitOfWork)new UnitOfWorkFactory().Create();
            app.CreatePerOwinContext<UnitOfWork>(() => dbContext);
            app.CreatePerOwinContext(() => IdentityFactory.CreateUserManager(dbContext));
            //Enable bearer authentication - this code adds JWT bearer authentication to the OWIN pipeline
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { "Any" },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                                                {
                                                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                                                }
            });

            //expose an OAuth endpoint so that the client can request a token (by passing a user name and password)
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,       //for debugging/PoC. Later disable insecure access
                TokenEndpointPath = new PathString("/oauth2/token"),    //End-point
                Provider = new CustomOAuthProvider(),                   //use custom provider and formatter
                AccessTokenFormat = new MSIdentityJwtFormat()
            });
        }
    }
}