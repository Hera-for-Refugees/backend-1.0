using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;

namespace Hera.Mobile.Api.Auth.Providers
{
    public class MobileClientAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return base.ValidateClientAuthentication(context);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            using (var unitOfWork = new Core.UnitOfWork.UnitOfWork())
            {
                var userInfo = new LoggedUserInfo { Id = 0, UserName = "Guest" };

                var user = unitOfWork.Repository<Data.Entity.Member>().GetBy(x => x.Mobile == context.UserName && x.Password == context.Password).FirstOrDefault();

                if (user != null)
                    userInfo = new LoggedUserInfo { Id = user.Id, UserName = context.UserName };


                if (userInfo.Id < 1)
                {
                    context.SetError("invalid_grant", "AuthenticationError");
                }
                else
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));
                    context.Validated(identity);
                }
            }
            return base.GrantResourceOwnerCredentials(context);
        }
    }

    public class LoggedUserInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}