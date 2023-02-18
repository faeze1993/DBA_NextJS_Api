using JwtApi.Redis;
using JwtApi.Redis.RedisModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace JwtApi.Jwt
{
    public class UpdateTokenExpireTimeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {

                var accessToken = HttpContext.Current.Request.Headers["Authorization"];
                if (accessToken != null)
                {
                    accessToken = accessToken.Substring(7); //bearer accessToken

                    var tokenData = UserDataRedisCacheService.Get(accessToken).FirstOrDefault();
                    if (tokenData != null)
                    {
                        var accessTokenExpireTime = Convert.ToDouble(ConfigurationManager.AppSettings["AccessTokenExpireTime"]);
                        tokenData.AccessTokenExpireTime = DateTime.Now.Add(TimeSpan.FromMinutes(accessTokenExpireTime));

                        var dataList = new List<UserData>();
                        dataList.Add(tokenData);
                        var mm = UserDataRedisCacheService.Set(accessToken, dataList);
                    }
                }
               

            }
            catch (Exception e)
            {

            }
        }
    }
}