using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using JwtApi.App_Start;
using JwtApi.Jwt;
using Owin;
using System;


[assembly: OwinStartup(typeof(OwinStartup))]
namespace JwtApi.App_Start
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
         
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/Login"),
                //زمان انقضای توکن رو یک روزه در نظر میگیریم 
                //همچنین دیتا در redis یک روزه ذخیره میشوند 
                //بعد از لاگین در دیتایی که در redis ذخیره میکنیم تایم تعیین میکنیم که مثلا تا ده دقیقه اگر غیر فعال بود نیاز به لاگین مجدد دارد 
                //در هر ریکوئست این زمان را آپدیت میکنیم --> BaseController,OnResultExecuted
                //در فیلتر JwtAuthorizeAttribute درصورتی که زمان درخواست بیشتر از زمان ذخیره شده در redis باشد پس زمان تمام شده و نیاز به لاگین دارد
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider()
            };

            //app.UseCors(CorsOptions.AllowAll);

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            // Api controllers with an [Authorize] attribute will be validated
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
      
    }
  
}