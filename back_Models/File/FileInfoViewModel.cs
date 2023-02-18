using back_Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.File
{
    public class FileInfoViewModel
    {
        public Guid stream_id { get; set; }
        public byte[] file_stream { get; set; }
        public string name { get; set; }
    }

    public class FileManagementViewModel
    {
        public Guid? Stream_id { get; set; }
        public string Directory { get; set; }
        public string Name { get; set; }
        public DateTime? CreatationDate { get; set; }
        public string PersianCreatationDate { get; set; }
        public double Size { get; set; }
        public bool IsDirectory { get; set; }
        public PaginateViewModel paginetedata { get; set; }
    }

    public class FileBreadCrumbViewModel
    {
        public Guid? StreamId { get; set; }
        public int? Level { get; set; }
        public string Name { get; set; }
    }

    public class FileSearchViewModel
    {
        public string SerachValue { get; set; }
    }

    public class UserFilesViewModel
    {
        public int? Id { get; set; }
        public Guid StreamId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public DateTime? CreatationDate { get; set; }
        public string PersianCreatationDate { get; set; }
        public double Size { get; set; }
    }
}
