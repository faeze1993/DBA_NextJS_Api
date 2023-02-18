using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Account
{
    public class RegisterUserApiViewModel
    {
        public string FName {get; set;}
        public string LName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public long Mobile { get; set; }
        public DateTime BirthDate { get; set; }
        public byte GenderId { get; set; }
        public string Username { get; set; }
        public string PassWord { get; set; }
        public string RepeatPassword { get; set; }
    }

    public class RecaptchaViewModel
    {
        public string CaptchaToken { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class RecaptchaResponseViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime challenge_ts { get; set; }
        public float score { get; set; }
        public List<string> ErrorCodes { get; set; }
        public bool Success { get; set; }
        public string hostname { get; set; }
    }
}
