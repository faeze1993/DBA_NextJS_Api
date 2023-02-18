using back_Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.Article
{
    public class SingleArticleViewModel
    {
        public bool testfromserver { get; set; }
        public ArticleViewModel Article { get; set; }
    }
    public class ArticleViewModel
    {
        public int? Id { get; set; }
        public int? Article_NextID { get; set; }
        public string Article_NextName { get; set; }
        public int? Article_PreID { get; set; }
        public string Article_PreName { get; set; }
        public bool IsDraft { get; set; }
        public bool IsEnable { get; set; }
        public bool IsTempSave { get; set; }
        public int? Article_CloneId { get; set; }
        public int? ParentId { get; set; }
        public int? AuthorId { get; set; }
        public short? TimeToRead { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorDescription { get; set; }
        public string Refrences { get; set; }
        public string Name { get; set; }
        public string Breadcrumbs { get; set; }
        public bool HasChild { get; set; }
        public int? NumberOfChild { get; set; }
        public long? Views { get; set; }
        public string LatinName { get; set; }
        public string KeyWords { get; set; }
        public string[] KeyWordsList { get; set; }
        public byte ArticleTypeId { get; set; }
        public string Body { get; set; }
        public string Summery { get; set; }
        public string UploadDate { get; set; }
        public int UpdateDate { get; set; }
        public string RefrenceList { get; set; }
        public DateTime UploadDateforOrderby { get; set; }
        public Stream ImageStream { get; set; }
        public string ImageUrl { get; set; }
        public int? TotalRows { get; set; }
        public PaginateViewModel paginetedata { get; set; }
        
    }

    public class ArticleTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public bool IsCountable { get; set; }
    }

    public class ArticleMenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<ArticleMenuViewModel> Childs { get; set; }
    }

    public class ArticleForMainPageViewModel
    {
        public List<ArticleViewModel> LatestArticles { get; set; }
        public List<ArticleViewModel> MostVisitedArticles { get; set; }
    }
    public class ArticleSearchViewModel
    {
        public string SerachValue { get; set; }
        public PaginateViewModel paginetedata { get; set; }
    }

    public class BreadCrumbViewModel
    {
        public int Id { get; set; }
        public short? Level { get; set; }
        public string Name { get; set; }
    }
}
