using back_Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.User
{
    public class EditUserByAdminViewModel
    {
        public int Id { get; set; }
        public bool HasVerify { get; set; }
        public List<ValueLabelViewModel> UserRoles { get; set; }
    }
}
