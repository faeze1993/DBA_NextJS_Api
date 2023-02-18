using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.PanelCustomValue
{
    public class PanelCustomValueViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Section { get; set; }
        public Guid? FileImageId { get; set; }
        public string ImageUrl { get; set; }
    }
}
