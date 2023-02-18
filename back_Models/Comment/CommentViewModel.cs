using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Models.Comment
{
    public class CommentViewModel
    {
        public int? Id { get; set; }
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public bool IsReplyByAdmin { get; set; }
        public bool IsConfirm { get; set; }
        public DateTime Date { get; set; }
        public string PersianDate { get; set; }
        public int? Comment_ReplyId { get; set; }
        public string UserImage { get; set; }
        public bool AdminOrPublisherComment { get; set; }

    }

   public class CommentRequest
    {
        public int ArticleId { get; set; }
        public int perSection { get; set; }
        public int currentSection { get; set; }
    }
}
