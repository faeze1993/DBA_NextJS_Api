using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using JwtApi.Redis;
using JwtApi.Redis.RedisModel;
using Services.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace JwtApi.Jwt
{
    public class CustomOAuthProvider: OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //context.OwinContext.Request.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            //if (context.ClientId == null)
            //{
            //    context.SetError("invalid_clientId", "client_Id is not set");
            //    return Task.FromResult<object>(null);
            //}
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // در jwt برای login enableCors نداریم
            //این مورد رو اضافه میکنیم که کاربر استفاده کننده از سرویسمون با خطای cors مواجه نشه
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });

            string msg;
            var user = UserService.FindUser(context.UserName, context.Password, "123", out msg);

            if (user == null)
            {

                context.SetError("invalid_grant", string.IsNullOrWhiteSpace(msg) ? "نام کاربری یا رمز عبور اشتباه است." : msg);
                return Task.FromResult<object>(null);
            }

           
            var identity = new ClaimsIdentity("JWT");

            //set Authintication
            identity.AddClaim(new Claim(ClaimTypes.Name, user.ID.ToString()));
            identity.AddClaim(new Claim("sub", user.ID.ToString()));

            
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         "audience", "123" /*user.ClientID*/
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

            return Task.FromResult<object>(null);

        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            var token = context.AccessToken;

            var accessTokenExpireTime = Convert.ToDouble(ConfigurationManager.AppSettings["AccessTokenExpireTime"]);

            //set token and userData in redis
            var dataList = new List<UserData>()
            {
                new UserData()
                {
                    Token = token,
                    AccessTokenExpireTime = DateTime.Now.Add(TimeSpan.FromMinutes(accessTokenExpireTime)),
                }
            };

            try
            {
                var mm = UserDataRedisCacheService.Set(token, dataList);
            }
            catch (Exception e)
            {

            }

            return base.TokenEndpointResponse(context);
        }
    }
}