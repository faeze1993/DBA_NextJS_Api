using API.JwtApi.Jwt;
using JwtApi.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace JwtApi.Jwt
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //actionContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //actionContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });

            if (skipAuthorization(actionContext))
            {
                return;
            }
            
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization == null)
            {
                // null token
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }

            var accessToken = actionContext.Request.Headers.Authorization.Parameter;
            if (string.IsNullOrWhiteSpace(accessToken) ||
                accessToken.Equals("undefined", StringComparison.OrdinalIgnoreCase))
            {
                // null token
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }
            var claimsIdentity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                // this is not our issued token
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }


            var tokenInRedis = UserDataRedisCacheService.Get(accessToken);
            if (!tokenInRedis.Any())
            {
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }

            //اگر زمان ریکوئست از زمان ثبت شده در 
            //redis
            //بیشتر بود؛ توکن از
            //redis
            //حذف شده و نیاز به لاگین مجدد میباشد
            var tokenTimeInRedis = tokenInRedis.FirstOrDefault().AccessTokenExpireTime;
            if (DateTime.Now > tokenTimeInRedis)
            {
                var deleted = UserDataRedisCacheService.RemoveTokenKeyFromRedis(accessToken);
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }


        }
        private static bool skipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        //-----------------------------------
        //public bool allowMuliple
        //{
        //    get { return false; }
        //}
        //public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        //{
        //    string authParameter = string.Empty;
        //    HttpRequestMessage request = context.Request;
        //    AuthenticationHeaderValue authentication = request.Headers.Authorization; 
        //    if(authentication == null)
        //    {
        //        context.ErrorResult = new AuthenticationFailureRequest("Missing Authorization Header", request);
        //        return;
        //    }
        //    if (authentication.Scheme != "Bearer")
        //    {
        //        context.ErrorResult = new AuthenticationFailureRequest("Invalid Authorization Scheme", request);
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(authentication.Parameter))
        //    {
        //        context.ErrorResult = new AuthenticationFailureRequest("Missing Token", request);
        //        return;
        //    }

           
        //    context.Principal = TokenManager.GetPrincipal(authentication.Parameter);
        //}

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = await context.Result.ExecuteAsync(cancellationToken);
            if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=localhost"));
            }
            context.Result = new ResponseMessageResult(result);
        }
    }

    public class AuthenticationFailureRequest : IHttpActionResult
    {
        public string ReasonPhrase;
        public HttpRequestMessage Request { get; set; }
        public AuthenticationFailureRequest(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }
        public HttpResponseMessage Execute()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            responseMessage.RequestMessage = Request;
            responseMessage.ReasonPhrase = ReasonPhrase;
            return responseMessage;
        }
    }
}