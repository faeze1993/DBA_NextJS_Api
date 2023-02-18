using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services.Library
{
    public sealed class EnvironmentVariable
    {
        public static int UserId
        {
            get
            {
                int.TryParse(HttpContext.Current?.User.Identity.Name, out var userId);
                return userId;
            }
        }
    }
}
