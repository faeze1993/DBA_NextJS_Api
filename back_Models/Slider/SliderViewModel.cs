using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.Slider
{
    public class SliderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LinkUrl { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public Guid ImageId { get; set; }
    }
}
