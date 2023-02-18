using back_Models.Common;
using back_Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.User
{
    public class UserViewModel
    {
		public int ID { get; set; }
		public string Username { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }
        public long Mobile { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string LastLoginDate { get; set; }

		public DateTime PasswordExpirationDate { get; set; }

		public DateTime LastPasswordChangedDate { get; set; }

		public DateTime CreationDate { get; set; }

		public bool IsOnLine { get; set; }

		public bool IsEnabled { get; set; }

		public DateTime LastEnabledDate { get; set; }

		public int FailedPasswordAttemptCount { get; set; }

		public string HintQuestion { get; set; }

		public string HintAnswer { get; set; }

		public bool ChangedPermission { get; set; }

		public DateTime FinishLockDatetime { get; set; }

		public string IPAddress { get; set; }

		public int VerificationCode { get; set; }

		public DateTime ExpireTimeVerificationCode { get; set; }

		public bool HasVerify { get; set; }
        public List<ValueLabelViewModel> UserRoles { get; set; }
    }
}
