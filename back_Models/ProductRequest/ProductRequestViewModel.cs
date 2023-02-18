using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.ProductRequest
{
    public class ProductRequestViewModel
    {
        public int? Id { get; set; }
        public string FullName { get; set; }
        public long Mobile { get; set; }
        public string Products { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string PersianDate { get; set; }
        public bool IsReplied { get; set; }
    }
}
