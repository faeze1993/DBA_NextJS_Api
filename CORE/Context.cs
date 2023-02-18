using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CORE
{
    partial class CoreDataContext
    {
        private bool SetContext = false;
        public static void SetDbSessionInfo(CoreDataContext db, int userId)
        {
           /* bool isAutoOpenConnection = false*/;

            if (db == null)
                throw new Exception("");

            //if (db.Connection.State == ConnectionState.Closed)
            //{
            //    //isAutoOpenConnection = true;
            //    db.Connection.Open();
            //}

            //if(db.SetContext == false)
            //{
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
            //    userName = db.Users.Where(el => el.ID == userId).Select(el => el.Username).SingleOrDefault() ?? "guest";
            //    var text = $"{userName}_{ipAddress}";
            //    db.SetContextInfo(text);

                db.SetContext = true;
            //}
           
        }
    }
}
