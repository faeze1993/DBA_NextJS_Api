using back_Models.Comment;
using CORE;
using Models.MessageClass;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Services.Comment
{
    public class CommentService
    {
        public static MessageClass InsertOrUpdate(CommentViewModel model, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var isAdmin = db.UserRoles.Where(el => el.UserID == userId).Any(el => el.RoleID == (int)CORE.Enums.Role.Admin || el.RoleID == (int)CORE.Enums.Role.Publisher);
            var commentModel = CreateCommentModel(model, isAdmin, userId);
            var mc = model.Id == null
                ? Insert(commentModel, isAdmin, ref db)
                : Update(commentModel, isAdmin, ref db);
            return mc;
        }
        private static CORE.Comment CreateCommentModel(CommentViewModel model, bool isAdmin, int userId)
        {
            var db = new CoreDataContext();

            var connection = db.Connection;
            connection.Open();
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
            var obj = cmd.ExecuteScalar();
            var anInt = (Int32)obj;

            var comment = new CORE.Comment()
            {
                ID = model.Id ?? anInt,
                ArticleID = model.ArticleId,
                UserID = userId,
                Date = DateTime.Now,
                Description = model.Description,
                Comment_ReplyID = model.Comment_ReplyId,
                IsConfirm = isAdmin,
                IsReplyByAdmin = model.IsReplyByAdmin
            };
            return comment;
        }
        private static MessageClass Insert(CORE.Comment item, bool isAdmin, ref CoreDataContext db)
        {

            var ec = new MessageClass();

            db.Comments.InsertOnSubmit(item);
            try
            {
                db.SubmitChanges();
                ec.Message = isAdmin ? "نظر شما با موفقیت ثبت گردید" : "نظر شما با موفقیت ثبت شد. بعد از تایید توسط کارشناسان به اشتراک گذاشته می شود.";
                ec.Status = "success";
            }
            catch (Exception e)
            {
                ec.Message = "خطایی رخ داده!" + e.Message;
                ec.Status = "error";
            }
            return ec;
        }
        private static MessageClass Update(CORE.Comment item, bool isAdmin, ref CoreDataContext db)
        {
            var ec = new MessageClass();
            //foreach (var item in items)
            //{
            var comment = db.Comments.SingleOrDefault(el => el.ID == item.ID);
            if (comment == null) return ec;
            //comment.ArticleID = item.ArticleID;
            //comment.UserID = item.UserID;
            //comment.Date = item.Date;
            comment.Description = item.Description;
            //comment.Comment_ReplyID = item.Comment_ReplyID;
            comment.IsConfirm = item.IsConfirm;
            comment.IsReplyByAdmin = item.IsReplyByAdmin;
            //}
            try
            {
                db.SubmitChanges();
                ec.Message = isAdmin ? "عملیات با موفقیت انجام شد" : "نظر شما با موفقیت ویرایش شد. بعد از تایید توسط کارشناسان به اشتراک گذاشته می شود.";
                ec.Status = "success";
            }
            catch (Exception e)
            {
                ec.Message = "خطایی رخ داده!" + e.Message;
                ec.Status = "error";
            }
            return ec;
        }
        public static MessageClass Delete(int id, int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var comment = db.Comments.Where(el => el.ID == id).Select(el => el).SingleOrDefault();
            db.Comments.DeleteOnSubmit(comment);
            try
            {
                db.SubmitChanges();
                ec.Message = "عملیات با موفقیت انجام شد";
                ec.Status = "success";
            }
            catch (Exception e)
            {
                ec.Message = "خطایی رخ داده!" + e.Message;
                ec.Status = "error";
            }
            return ec;
        }
        public static IQueryable<CommentViewModel> GetCommentsGridData(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from comment in db.Comments
                     join article in db.Articles on comment.ArticleID equals article.ID
                     join user in db.Users on comment.UserID equals user.ID
                     select new CommentViewModel
                     {
                         Id = comment.ID,
                         Description = comment.Description,
                         Date = comment.Date,
                         PersianDate = comment.Date == null ? null : CalendarService.ConvertToPersian(comment.Date).ToString("HH:mm yyyy/MM/dd"),
                         ArticleId = comment.ArticleID,
                         ArticleName = article.Name,
                         UserId = comment.UserID,
                         UserName = user.Username,
                         IsConfirm = comment.IsConfirm
                     });
            
            return q;
        }

        //public static List<CommentViewModel> GetComments(int articleId, int userId)
        //{
        //    var db = new CoreDataContext();
        //    var query = db.Comments.Where(el => el.ArticleID == articleId && el.Comment_ReplyID == null && (el.IsConfirm == true || el.UserID == userId));

        //    var comments = (from q in query
        //                    join people in db.Peoples on q.UserID equals people.ID
        //                    join fileView in db.FilesViews on people.Files_ImageID equals fileView.stream_id into lFileView
        //                    from fileView in lFileView.DefaultIfEmpty()
        //                    join userRole in db.UserRoles on q.UserID equals userRole.UserID
        //                    join role in db.Roles on userRole.RoleID equals role.ID
        //                    group new
        //                    {
        //                        roleId = role.ID,
        //                    }
        //                    by new
        //                    {
        //                        q.UserID,
        //                        q.ID,
        //                        q.IsConfirm,
        //                        q.Description,
        //                        q.Date,
        //                        people.FirstName,
        //                        people.LastName,
        //                        q.IsReplyByAdmin,
        //                        fileView.directory,
        //                        q.ArticleID
        //                    } into grp
        //                    select new CommentViewModel()
        //                    {
        //                        Id = grp.Key.ID,
        //                        IsConfirm = grp.Key.IsConfirm,
        //                        AdminOrPublisherComment = grp.Sum(el => el.roleId == (int)CORE.Enums.Role.User ? 0 : 1) == 0 ? false : true,
        //                        Description = grp.Key.Description,
        //                        Date = grp.Key.Date,
        //                        PersianDate = CalendarService.ConvertToPersian(grp.Key.Date).ToString("HH:mm yyyy/MM/dd"),
        //                        FullName = grp.Key.FirstName + " " + grp.Key.LastName,
        //                        IsReplyByAdmin = grp.Key.IsReplyByAdmin,
        //                        UserImage = grp.Key.directory.Replace(@"\", "/"),
        //                        ArticleId = grp.Key.ArticleID,
        //                        UserId = grp.Key.UserID
        //                    }).OrderByDescending(q => q.Date).ToList();

        //    var commentsChild = (from qComment in query
        //                         join comment in db.Comments.Select(el => el) on qComment.ID equals comment.Comment_ReplyID
        //                         join people in db.Peoples on comment.UserID equals people.ID
        //                         join fileView in db.FilesViews on people.Files_ImageID equals fileView.stream_id into lFileView
        //                         from fileView in lFileView.DefaultIfEmpty()
        //                         join userRole in db.UserRoles on qComment.UserID equals userRole.UserID
        //                         join role in db.Roles on userRole.RoleID equals role.ID
        //                         group new { roleId = role.ID, }
        //                         by new
        //                         {
        //                             comment.UserID,
        //                             comment.ID,
        //                             comment.IsConfirm,
        //                             comment.Description,
        //                             comment.Date,
        //                             people.FirstName,
        //                             people.LastName,
        //                             comment.IsReplyByAdmin,
        //                             fileView.directory,
        //                             comment.ArticleID,
        //                             comment.Comment_ReplyID
        //                         }
        //                         into grp
        //                         select new CommentViewModel()
        //                         {
        //                             Id = grp.Key.ID,
        //                             IsConfirm = grp.Key.IsConfirm,
        //                             AdminOrPublisherComment = grp.Sum(el => el.roleId == (int)CORE.Enums.Role.User ? 0 : 1) == 0 ? false : true,
        //                             Description = grp.Key.Description,
        //                             Date = grp.Key.Date,
        //                             PersianDate = CalendarService.ConvertToPersian(grp.Key.Date).ToString("HH:mm yyyy/MM/dd"),
        //                             FullName = grp.Key.FirstName + " " + grp.Key.LastName,
        //                             IsReplyByAdmin = grp.Key.IsReplyByAdmin,
        //                             Comment_ReplyId = grp.Key.Comment_ReplyID,
        //                             UserImage = grp.Key.directory.Replace(@"\", "/"),
        //                             ArticleId = grp.Key.ArticleID,
        //                             UserId = grp.Key.UserID
        //                         }).OrderByDescending(x => x.Date).ToList();


        //    comments.AddRange(commentsChild);
        //    return comments;
        //    //return new List<CommentViewModel>();
        //}


        public static List<CommentViewModel> GetComments(int articleId,int perSection , int currentSection, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var fromStep = (perSection * (currentSection - 1));
            var stepNumber = perSection;
            var query = db.CommentPagination(articleId, fromStep, stepNumber, userId);
            var result = query.Select(el => new CommentViewModel
            {
                Id = el.Cmnt_ID,
                IsConfirm = el.cmnt_IsConfirm,
                AdminOrPublisherComment = el.cmnt_AdminOrPublisher == 1,
                Description = el.cmnt_Description,
                Date = el.cmnt_Date,
                PersianDate = el.cmnt_Date == null ? null : CalendarService.ConvertToPersian(el.cmnt_Date).ToString(),
                FullName = el.cmnt_FullName,
                Comment_ReplyId = el.reply_ID,
                UserImage = el.cmnt_UserImage?.Replace(@"\", "/"),
                UserId = el.cmnt_UserID,
                ArticleId = el.cmnt_ArticleID,
            }).OrderByDescending(x => x.Date).ToList();
            return result;
        }
        public static MessageClass ToggleConfirm(int id, int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var comment = db.Comments.SingleOrDefault(el => el.ID == id);
            if (comment == null) { ec.Message = "کامنت مورد نظر حذف گردیده است";ec.Status = "error"; };
            comment.IsConfirm = !comment.IsConfirm;

            try
            {
                db.SubmitChanges();
                ec.Message = "عملیات با موفقیت انجام شد";
                ec.Status = "success";
            }
            catch (Exception e)
            {
                ec.Message = "خطایی رخ داده!" + e.Message;
                ec.Status = "error";
            }
            return ec;
        }
    }
}
