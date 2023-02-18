using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.ConectUs
{
    public class ConectUsViewModel
    {
        public int? Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool IsReplied { get; set; }
        public DateTime Date { get; set; }
        public string PersianDate { get; set; }
    }
}
