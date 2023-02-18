using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.Common
{
    public class PaginateViewModel
    {
        public int perpage { get; set; }
        public int currntpage { get; set; }
        public int skip { get; set; }
        public string searchValue { get; set; }
    }

    public class PaginationViewModel<T>
    {
        public List<T> ItemList { get; set; }
        public int? TotalItems { get; set; }
        public bool ResponsFromServer { get; set; }
    }
}
