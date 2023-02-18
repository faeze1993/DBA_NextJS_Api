using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.User
{
    public class UserProfileDataViewModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public bool IsAdmin { get; set; }
        public string CreationDate { get; set; }
    }

    public class CheckVerifyCodeViewModel
    {
        public int Code { get; set; }
        public string Username { get; set; }
    }

    public class CheckMobileNumberViewModel 
    {
        public long Mobile { get; set; }
    }

}
