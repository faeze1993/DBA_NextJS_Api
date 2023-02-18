using back_Models.Common;
using back_Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.PanelCustomValue
{
    public class PanelMenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NavigateUrl { get; set; }
        public int RoleId { get; set; }
        public string IconDirectoty { get; set; }
        public Guid? IconId { get; set; }
        public List<ValueLabelViewModel> PanelMenuRoles { get; set; }
    }
}
