using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace Hera.Mobile.Api.Auth.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Create(context);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            object owinCollection;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection", out owinCollection);
            var grantType = ((FormCollection)owinCollection)?.GetValues("grant_type").FirstOrDefault();


            var resultIsNull = false;

            if (grantType == null || grantType.Equals("refresh_token"))
            {
                var username = context.Ticket.Identity.FindFirst("sub").Value;
                using (var db = new Data.Entity.HeraEntities())
                {
                    var member = db.Member.FirstOrDefault(x => x.Mobile == username);
                    if (member != null && member.IsApproved)
                    {
                        context.Ticket.Properties.ExpiresUtc = DateTime.Now.AddDays(3);
                        context.SetToken(context.SerializeTicket());
                    }
                    else
                        resultIsNull = true;
                }
            }
            else
            {
                context.Ticket.Properties.ExpiresUtc = DateTime.Now.AddDays(3);
                context.SetToken(context.SerializeTicket());
            }
            if (resultIsNull)
            {
                if (context.Ticket.Properties.ExpiresUtc <= DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    context.Response.ReasonPhrase = "unauthorized";
                    return;
                }
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Receive(context);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket == null)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
                return;
            }

            if (context.Ticket.Properties.ExpiresUtc <= DateTime.UtcNow)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "unauthorized";
                return;
            }

            context.SetTicket(context.Ticket);
        }
    }
}