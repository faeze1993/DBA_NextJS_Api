
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace JwtApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }

        //protected void Application_BeginRequest()
        //{

        //    var db = new CoreDataContext(); 
        //    string ipAddress = string.Empty;
        //    string address = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(address))
        //    {
        //        string[] addresses = address.Split(',');
        //        if (addresses.Length != 0)
        //        {
        //            ipAddress = addresses[0];
        //        }
        //    }
        //    else
        //    {
        //        ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }

        //    string userName;
        //    userName = /*db.Users.Where(el => el.ID == userId).Select(el => el.Username).SingleOrDefault() ??*/ "guest";
        //    var text = $"{userName}_{ipAddress}";
        //    db.SetContextInfo(text);
        //}
    }
}
