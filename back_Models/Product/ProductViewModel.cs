using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string LinkUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public Guid FileInfo_ImageId { get; set; }

    }
}
