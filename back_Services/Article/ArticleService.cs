using back_Models.Article;
using back_Models.File;
using back_Services.File;
using CORE;
using Models.MessageClass;
using Newtonsoft.Json;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CORE.Enums;
using System.Data.SqlClient;
using System.Data.Linq.SqlClient;
using System.Text.RegularExpressions;
using Ganss.XSS;
using System.Data.Linq;
using back_Models.Common;

namespace back_Services.Article
{
    public class ArticleService
    {
       
        public static MessageClass Save(int userId)
        {
            var db = new CoreDataContext();
            db.Connection.Open();
            db.Transaction = db.Connection.BeginTransaction();
            var tr = db.Transaction;

            var mc = new MessageClass();
            var fileData = new Dictionary<Guid, string>();
            var articleFiles = new List<CORE.ArticleFile>();
            var sanitizer = new HtmlSanitizer();
            var test1 = HttpContext.Current.Request.Params["formdata"];

            var model = JsonConvert.DeserializeObject<ArticleViewModel>(HttpContext.Current.Request["formdata"]);

            if (model.ArticleTypeId == (int)CORE.Enums.ArticleType.Article)
            {
                var links = SearchArtcileBodyUsingRegex(model.Body);

                foreach (var link in links)
                {
                    var strLink = link.ToString();
                    var startIndex = link.ToString().IndexOf("(") + 1;
                    var length = (link.ToString().Length) - 1;
                    var subString = strLink.Substring(startIndex);
                    var unc_path = subString.Substring(0, subString.Length - 1);

                    var fileID = db.FilesViews.SingleOrDefault(el => el.directory.Replace(@"\", "/") == unc_path).stream_id;
                    fileData.Add(fileID, unc_path);
                }

            }

            var isNew = model.Id == null || model.Id == 0;

            if (isNew)
            {
                var cmd = db.Connection.CreateCommand();
                cmd.Transaction = tr;
                cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
                var obj = cmd.ExecuteScalar();
                var anInt = (Int32)obj;

                var newArticle = new CORE.Article
                {
                    ID = anInt,
                    Name = model.Name,
                    IsDraft = model.IsTempSave,
                    Enable = model.IsEnable,
                    LatinNm = model.LatinName,
                    User_AuthorID = userId,
                    ArticleTypeID = model.ArticleTypeId,
                    Article_NextID = model.Article_NextID,
                    Article_PreID = model.Article_PreID,
                    Body = model.Body,
                    Summery = model.Summery,
                    Refrences = model.RefrenceList,
                    UploadDate = DateTime.Now,
                    LastPublishDate = DateTime.Now,
                    //Files_ImageID = model.imageurlId,
                    KeyWords = model.KeyWords,
                    ParentID = model.ParentId,
                    TimeToRead = model.TimeToRead
                };

                db.Articles.InsertOnSubmit(newArticle);

                foreach (var item in fileData)
                {
                    cmd.Transaction = tr;
                    cmd = db.Connection.CreateCommand();
                    cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
                    var objj = cmd.ExecuteScalar();
                    var fileIdInt = (Int32)objj;
                    var articleFile = new CORE.ArticleFile() { ID = fileIdInt, ArticleID = anInt, FilesID = item.Key, LinkAddRess = item.Value };
                    articleFiles.Add(articleFile);
                };

                db.ArticleFiles.InsertAllOnSubmit(articleFiles);

                db.SetNextPreArticle(newArticle.ID, model.Article_PreID, model.Article_NextID);

                mc.Data = new { Id = newArticle.ID };
            }
            else
            {
                //در حالت ویرایش - اگر
                //IsTempSave = true
                //باشد یک
                //clone
                //از سطر اصلی مقاله ایجاد می شود و با شناسه جدید درج می شود. شناسه ی این سطر جعلی در فیلد
                //Article_CloneId
                //سطر مقاله اصلی درج می شود .
                //تا زمانی که نویسند کلید ثبت را بزند و مقاله برای پابلیش نهایی اماده باشد. در این حالت دیتای سطر کلون جایگزین سطر اصلی میشود

                //در هر بار ویرایش اگر سطر جعلی به ازای سطر اصلی وجود داشت دیتا ویرایش شده در سطر جعلی می نشیند.

                CORE.Article dbArticle = null;

                //اگر ثبت موقت بود ، ویرایش روی مقاله موقت ثبت می شود   
                // اگر ثبت نهایی بود ، ویرایش روی مقاله اصلی ثبت می شود

                if (model.IsTempSave && model.Id > 0)
                {
                    dbArticle = db.Articles.SingleOrDefault(el => el.ID == model.Id);
                }
                else if (model.IsTempSave && model.Id < 0)
                {
                    dbArticle = db.Articles.SingleOrDefault(el => el.ID == model.Id);
                }
                else if (!model.IsTempSave && model.Id > 0)
                {
                    dbArticle = db.Articles.SingleOrDefault(el => el.ID == model.Id);
                }
                else if (!model.IsTempSave && model.Id < 0)
                {
                    dbArticle = db.Articles.SingleOrDefault(el => el.Article_CloneID == model.Id);
                    if (!dbArticle.Enable && false/*&& !SkipOnWarning*/)//felan piade sazi nashode ast
                    {
                        mc.Message = "مقاله اصلی در حالت غیر فعال قرار دارد، با ثبت نهایی مقاله از حالت غیر فعال خارج می شود";
                        mc.Status = "warning";
                        return mc;
                    }

                }
                else
                {
                    dbArticle = null;
                }

                if (dbArticle == null)
                {
                    mc.Message = "این سطر در بانک وجود ندارد";
                    mc.Status = "error";
                    db.Transaction.Rollback();
                    return mc;
                };

                dbArticle.Name = model.Name;
                dbArticle.LatinNm = model.LatinName;
                dbArticle.Body = model.Body;
                dbArticle.IsDraft = model.IsTempSave;
                dbArticle.Enable = model.IsEnable;
                dbArticle.Summery = model.Summery;
                dbArticle.KeyWords = model.KeyWords;
                dbArticle.Refrences = model.RefrenceList;
                dbArticle.TimeToRead = model.TimeToRead;
                dbArticle.LastPublishDate = DateTime.Now;
                dbArticle.User_LastEditID = userId;
                dbArticle.Article_CloneID = null;
                dbArticle.Article_NextID = model.Article_NextID;
                dbArticle.Article_PreID = model.Article_PreID;

                var filesForDelete = db.ArticleFiles.Where(el => el.ArticleID == dbArticle.ID).Select(el => el);
                if (filesForDelete.Count() > 0)
                {
                    db.ArticleFiles.DeleteAllOnSubmit(filesForDelete);
                }

                try
                {
                    db.SubmitChanges();
                    mc.Message = "عملیات با موفقیت انجام شد";
                    mc.Status = "success";
                    mc.Data = new { Id = dbArticle.ID };

                }
                catch (Exception e)
                {
                    mc.Message = "خطایی رخ داده!" + e.Message;
                    mc.Status = "error";
                }

                foreach (var item in fileData)
                {
                    //var connection = db.Connection;
                    //connection.Open();
                    var cmd = db.Connection.CreateCommand();
                    cmd.Transaction = tr;
                    cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
                    var objj = cmd.ExecuteScalar();
                    var fileIdInt = (Int32)objj;
                    var articleFile = new CORE.ArticleFile() { ID = fileIdInt, ArticleID = dbArticle.ID, FilesID = item.Key, LinkAddRess = item.Value };
                    articleFiles.Add(articleFile);
                }
                db.ArticleFiles.InsertAllOnSubmit(articleFiles);

                if (!model.IsTempSave && model.Id < 0)
                {
                    var cloneArticle = db.Articles.SingleOrDefault(el => el.ID == model.Id);
                    db.Articles.DeleteOnSubmit(cloneArticle);
                }

                db.SetNextPreArticle(dbArticle.ID, model.Article_PreID, model.Article_NextID);
            };

            try
            {
                db.SubmitChanges();
                CoreDataContext.SetDbSessionInfo(db, userId);
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";

            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!" + e.Message;
                mc.Status = "error";
            }

            //ApllayBeforeAndNextIdForSelectedArticles();

            if (mc.Status != "error")
            {
                db.Transaction.Commit();
            }
            else
            {
                db.Transaction.Rollback();
            }
            return mc;
        }

        private static MessageClass ApllayBeforeAndNextIdForSelectedArticles(int articleId, int prearticleId, int nextArticleId, ref CoreDataContext db)
        {
            var ec = new MessageClass();
            return ec;
        }
        public static MessageClass ToggleEnable(int id, int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var article = db.Articles.SingleOrDefault(el => el.ID == id && !el.IsDraft);
            if (article == null) { ec.Message = "مقاله مورد نظر یافت نشد"; ec.Status = "error"; };

            article.Enable = !article.Enable;

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
        //private static MessageClass CreateDBModel()
        //{

        //}
        //private static MessageClass InsertArticle()
        //{
        //    var mc = new MessageClass();
        //    return mc;
        //}
        //private static MessageClass UpdateArticle()
        //{
        //    var mc = new MessageClass();
        //    return mc;
        //}
        public static IQueryable<ArticleViewModel> GetAllArticlesForDashboard(int? parentId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var parentID = parentId == 0 ? null : parentId;
            var q = (from article in db.Articles
                     join people in db.Peoples on article.User_AuthorID equals people.ID
                     join fileView in db.FilesViews on article.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     where article.ID > 0
                     select new ArticleViewModel
                     {
                         Id = article.ID,
                         ParentId = article.ParentID,
                         Name = article.Name,
                         ArticleTypeId = article.ArticleTypeID,
                         UploadDateforOrderby = article.UploadDate,
                         UploadDate = CalendarService.ConvertToPersian(article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                         AuthorName = people.FirstName + " " + people.LastName,
                         HasChild = db.Articles.Any(el => el.ParentID == article.ID),
                         Article_CloneId = article.Article_CloneID,
                         IsDraft = article.IsDraft,
                         IsEnable = article.Enable,
                     }).OrderByDescending(el => el.UploadDateforOrderby);

            if (parentID != null && parentID != 0)
            {
                var result = q.Where(el => el.ParentId == parentID);
                return result;
            }
            else if (parentID == null || parentID == 0)
            {
                var result = q.Where(el => el.ParentId == null);
                return result;
            }
            return q;
        }
        public static IQueryable<ValueLabelViewModel> GetAllArticlesForDropdown(int? parentId)
        {
            var db = new CoreDataContext();
            var parentID = parentId == 0 ? null : parentId;
            var q = db.Articles.Where(el => !el.IsDraft).Select(el => el);

            if (parentID != null && parentID != 0)
            {
                q = q.Where(el => el.ParentID == parentID);
            }
            else if (parentID == null || parentID == 0)
            {
                q = q.Where(el => el.ParentID == null);
            }

            var result = q.Select(el => new ValueLabelViewModel
            {
                value = el.ID,
                label = el.Name
            });
            return result;
        }
        public static ArticleViewModel GetArticlesForEdit(int articleId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            int conditionArticleId = articleId;
            var dbarticle = db.Articles.SingleOrDefault(el => el.ID == articleId);
            if (dbarticle == null)
                return new ArticleViewModel();

            if (dbarticle.IsDraft || dbarticle.ArticleTypeID == (int)CORE.Enums.ArticleType.ArticleGroup)
            {
                conditionArticleId = dbarticle.ID;
            }
            else if (!dbarticle.IsDraft && dbarticle.Article_CloneID != null)
            {
                conditionArticleId = (int)dbarticle.Article_CloneID;
            }
            else if (!dbarticle.IsDraft && dbarticle.Article_CloneID == null && dbarticle.ArticleTypeID == (int)CORE.Enums.ArticleType.Article)
            {
                conditionArticleId = db.Clone_Article(articleId, userId).SingleOrDefault().Article_CloneID.Value;
            }
            else
            {
                return new ArticleViewModel();
            }

            var q = (from article in db.Articles
                     join articlePre in db.Articles on article.Article_PreID equals articlePre.ID into lArticlePre
                     from articlePre in lArticlePre.DefaultIfEmpty()
                     join articleNext in db.Articles on article.Article_NextID equals articleNext.ID into lArticleNext
                     from articleNext in lArticleNext.DefaultIfEmpty()
                     join people in db.Peoples on article.User_AuthorID equals people.ID
                     join fileView in db.FilesViews on article.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     where article.ID == conditionArticleId
                     select new ArticleViewModel
                     {
                         Id = article.ID,
                         Article_NextID = article.Article_NextID,
                         Article_NextName = articleNext.Name,
                         Article_PreID = article.Article_PreID,
                         Article_PreName = articlePre.Name,
                         ParentId = article.ParentID,
                         Name = article.Name,
                         LatinName = article.LatinNm,
                         KeyWords = article.KeyWords,
                         Body = article.Body,
                         Summery = article.Summery,
                         ArticleTypeId = article.ArticleTypeID,
                         UploadDateforOrderby = article.UploadDate,
                         UploadDate = CalendarService.ConvertToPersian(article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                         AuthorId = article.User_AuthorID,
                         AuthorName = people.FirstName + " " + people.LastName,
                         HasChild = db.Articles.Any(el => el.ParentID == article.ID),
                         TimeToRead = article.TimeToRead,
                         IsDraft = article.IsDraft,
                         Refrences = article.Refrences,
                         IsEnable = article.Enable
                     }).SingleOrDefault();

            return q;
        }
        public static IQueryable<ArticleViewModel> GetAllArticlesForDashboardUsingSearch(string searchValue, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var articles = (from article in db.Articles
                            join findArticle in db.ArticleSearch(searchValue, 10) on article.ID equals findArticle.ID
                            orderby findArticle.Rank ascending
                            select article);

            var q = (from article in articles
                     join people in db.Peoples on article.User_AuthorID equals people.ID
                     join fileView in db.FilesViews on article.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                         //where article.ParentID == parentId
                     select new ArticleViewModel
                     {
                         Id = article.ID,
                         ParentId = article.ParentID,
                         Name = article.Name,
                         LatinName = article.LatinNm,
                         KeyWords = article.KeyWords,
                         Body = article.Body,
                         Summery = article.Summery,
                         ArticleTypeId = article.ArticleTypeID,
                         UploadDateforOrderby = article.UploadDate,
                         UploadDate = CalendarService.ConvertToPersian(article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                         AuthorId = article.User_AuthorID,
                         AuthorName = people.FirstName + " " + people.LastName,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),/*"http://localhost/pic/" + fileView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/"),//dar publish bayad taghir konad*/
                         HasChild = db.Articles.Any(el => el.ParentID == article.ID)

                     }).OrderByDescending(el => el.UploadDateforOrderby);


            return q;
        }
        public static PaginationViewModel<ArticleViewModel> GetAllArticles(int perPage, int currentPage, int? articleId, int? authorId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var result = new PaginationViewModel<ArticleViewModel>();
            var fromStep = (perPage * (currentPage - 1));
            var stepNumber = perPage;
            var query = db.ArticlePagination(fromStep, stepNumber, authorId, articleId);
            result.ItemList = query.Select(el => new ArticleViewModel
            {
                Id = el.ID,
                ParentId = el.ParentID,
                Name = el.Name,
                Summery = el.Summery,
                AuthorName = el.AuthorName,
                UploadDate = CalendarService.ConvertToPersian(el.UploadDate).ToString("yyyy/MM/dd"),
                HasChild = el.ParentChildCount > 0,
                NumberOfChild = el.ParentChildCount,
                TotalRows = el.Rows,
                ArticleTypeId = el.ArticleTypeID,
                Views = el.Views
            }).ToList();

            result.TotalItems = result.ItemList.Select(el => el.TotalRows).FirstOrDefault();
            result.ResponsFromServer = true;
            return result;
        }

        //Get All Articles that can display in archive page  
        public static List<ArticleViewModel> GetAllArticles2(int? articleId)
        {
            var db = new CoreDataContext();
            var result = new List<ArticleViewModel>();
            IQueryable<CORE.Article> q;

            var query = (from article in db.Articles
                         join people in db.Peoples on article.User_AuthorID equals people.ID
                         where !article.IsDraft && article.Enable
                         select article);
            if (articleId == null)
            {
                q = query.Where(el => el.ParentID == null);
            }
            else
            {
                q = query.Where(el => el.ParentID == articleId);
            };
            result = q.Select(el => new ArticleViewModel
            {
                Id = el.ID,
                ParentId = el.ParentID,
                Name = el.Name,
                Summery = el.Summery,
                //AuthorName = el.AuthorName,
                UploadDate = CalendarService.ConvertToPersian(el.UploadDate).ToString("yyyy/MM/dd"),
                //HasChild = el.ParentChildCount > 0,
                //NumberOfChild = el.ParentChildCount,
                //TotalRows = el.Rows,
                ArticleTypeId = el.ArticleTypeID,
                Views = el.Views
            }).ToList();


            return result;
        }

        //Get All Articles that can display in archive page 2  
        public static string GetAllArticleTrees(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            //var result = new PaginationViewModel<ArticleViewModel>();
            //var query = db.Articles.Where(el => el.Enable && !el.IsDraft);
            //result.ItemList = query.Select(el => new ArticleViewModel
            //{
            //    Id = el.ID,
            //    ParentId = el.ParentID,
            //    Name = el.Name,
            //    Summery = el.Summery,
            //    AuthorName = el.AuthorName,
            //    UploadDate = CalendarService.ConvertToPersian(el.UploadDate).ToString("yyyy/MM/dd"),
            //    HasChild = el.ParentChildCount > 0,
            //    NumberOfChild = el.ParentChildCount,
            //    TotalRows = el.Rows,
            //    ArticleTypeId = el.ArticleTypeID,
            //    Views = el.Views
            //}).ToList();

            //result.TotalItems = result.ItemList.Select(el => el.TotalRows).FirstOrDefault();
            //result.ResponsFromServer = true;

            return db.ViewAllTrees.Select(el => el.Trees).SingleOrDefault();
        }

        //Get All Articles that can display in article page  
        public static List<ArticleViewModel> GetAllArticleforArticlePath(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var result = new List<ArticleViewModel>();
            var query = db.Articles.Where(el => el.Enable && !el.IsDraft && el.ArticleTypeID == 2);
            result = query.Select(el => new ArticleViewModel
            {
                Id = el.ID,
                ParentId = el.ParentID,
                Name = el.Name,
                Summery = el.Summery,
                Body = el.Body,
                UploadDate = CalendarService.ConvertToPersian(el.UploadDate).ToString("yyyy/MM/dd"),
                ArticleTypeId = el.ArticleTypeID,
                Views = el.Views
            }).ToList();

            return result;
        }
        //Get All Articles on AuthrId that can display in Authr page 
        public static List<ArticleViewModel> GetAllArticlesOnAuthrId(int authrId)
        {
            var db = new CoreDataContext();
            var result = new List<ArticleViewModel>();
            IQueryable<CORE.Article> q;

            var query = (from article in db.Articles
                         join people in db.Peoples on article.User_AuthorID equals people.ID
                         where !article.IsDraft && article.Enable && article.User_AuthorID == authrId
                         select article);
           
            result = query.Select(el => new ArticleViewModel
            {
                Id = el.ID,
                ParentId = el.ParentID,
                Name = el.Name,
                Summery = el.Summery,
                //AuthorName = el.AuthorName,
                UploadDate = CalendarService.ConvertToPersian(el.UploadDate).ToString("yyyy/MM/dd"),
                //HasChild = el.ParentChildCount > 0,
                //NumberOfChild = el.ParentChildCount,
                //TotalRows = el.Rows,
                ArticleTypeId = el.ArticleTypeID,
                Views = el.Views
            }).ToList();


            return result;
        }
        public static ArticleForMainPageViewModel GetAllArticlesForMainPage(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var query = from article in db.Articles
                        join people in db.Peoples on article.User_AuthorID equals people.ID
                        where article.ArticleTypeID == (int)CORE.Enums.ArticleType.Article && article.IsDraft == false && article.Enable
                        select new { article, people };

            var model = new ArticleForMainPageViewModel
            {
                LatestArticles = query.Select(el => new ArticleViewModel
                {
                    Id = el.article.ID,
                    ParentId = el.article.ParentID,
                    Name = el.article.Name,
                    Summery = el.article.Summery,
                    UploadDateforOrderby = el.article.UploadDate,
                    UploadDate = CalendarService.ConvertToPersian(el.article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                    AuthorName = el.people.FirstName + " " + el.people.LastName,
                    Views = el.article.Views
                }).OrderByDescending(el => el.UploadDateforOrderby).Take(8).ToList(),


                MostVisitedArticles = query.Select(el => new ArticleViewModel
                {
                    Id = el.article.ID,
                    ParentId = el.article.ParentID,
                    Name = el.article.Name,
                    Summery = el.article.Summery,
                    UploadDateforOrderby = el.article.UploadDate,
                    UploadDate = CalendarService.ConvertToPersian(el.article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                    AuthorName = el.people.FirstName + " " + el.people.LastName,
                    Views = el.article.Views
                }).OrderBy(el => el.UploadDateforOrderby).Take(8).ToList(),
            };


            return model;
        }
        public static ArticleViewModel GetArticleDetail(int articleId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from article in db.Articles
                     join articlePre in db.Articles on article.Article_PreID equals articlePre.ID into lArticlePre
                     from articlePre in lArticlePre.DefaultIfEmpty()
                     join articleNext in db.Articles on article.Article_NextID equals articleNext.ID into lArticleNext
                     from articleNext in lArticleNext.DefaultIfEmpty()
                     join people in db.Peoples on article.User_AuthorID equals people.ID
                     join fileView in db.FilesViews on people.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     where article.ID == articleId && article.IsDraft == false && article.Enable
                     select new ArticleViewModel
                     {
                         Id = article.ID,
                         ParentId = article.ParentID,
                         Name = article.Name,
                         LatinName = article.LatinNm,
                         KeyWords = article.KeyWords,
                         KeyWordsList = article.KeyWords.Split(','),
                         Body = article.Body,
                         Views = article.Views == null ? 0 : article.Views,
                         UploadDate = CalendarService.ConvertToPersian(article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                         UpdateDate = (int)(DateTime.Now - (DateTime)article.LastPublishDate).TotalDays,
                         AuthorName = people.FirstName + " " + people.LastName,
                         AuthorId = article.User_AuthorID,
                         AuthorImage = fileView.directory.Replace(@"\", "/"),
                         AuthorEmail = people.E_mail,
                         AuthorDescription = people.Description,
                         TimeToRead = article.TimeToRead,
                         Refrences = article.Refrences,
                         Summery = article.Summery,
                         Article_NextID = article.Article_NextID,
                         Article_NextName = articleNext.Name,
                         Article_PreID = article.Article_PreID,
                         Article_PreName = articlePre.Name
                     }).SingleOrDefault();

            return q;
        }
        public static MessageClass DeleteArticle(int articleId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var comments = db.Comments.Where(el => el.ArticleID == articleId).Select(el => el).ToList();
            if (comments.Count > 0)
            {
                db.Comments.DeleteAllOnSubmit(comments);
            }

            var articleFiles = db.ArticleFiles.Where(el => el.ArticleID == articleId).Select(el => el).ToList();
            if (articleFiles.Count > 0)
            {
                db.ArticleFiles.DeleteAllOnSubmit(articleFiles);
            }

            var articleCloneId = db.Articles.SingleOrDefault(el => el.ID == articleId).Article_CloneID;
            if (articleCloneId != null)
            {
                var articleClone = db.Articles.SingleOrDefault(el => el.ID == articleCloneId);
                db.Articles.DeleteOnSubmit(articleClone);
            }

            var isCloneArticle = db.Articles.Any(el => el.Article_CloneID == articleId);
            if (isCloneArticle)
            {
                var mainArticle = db.Articles.SingleOrDefault(el => el.Article_CloneID == articleId);
                mainArticle.Article_CloneID = null;
            }

            var article = db.Articles.SingleOrDefault(el => el.ID == articleId);
            db.Articles.DeleteOnSubmit(article);

            try
            {
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";
            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!" + e.Message;
                mc.Status = "error";
            }

            return mc;
        }
        public static List<ArticleTypeViewModel> GetArticleTypes(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = db.ArticleTypes.Select(el => new ArticleTypeViewModel
            {
                Id = el.ID,
                Name = el.Name
            }).ToList();
            return q;
        }
        public static string GetAllArticleMenu(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var menu = db.ArticleTrees.Select(el => el.TextCompressed).SingleOrDefault();
            //ArticleMenuViewModel newNode = null;
            //var result = MakeTree(menus, newNode);
            //var model = JsonConvert.SerializeObject(result);
            return menu;
        }

        //don't use
        private static List<ArticleMenuViewModel> MakeTree(IEnumerable<CORE.Article> list, ArticleMenuViewModel parentNode)
        {
            List<ArticleMenuViewModel> treeViewList = new List<ArticleMenuViewModel>();
            var nodes = list.Where(x => parentNode == null ? x.ParentID == null : x.ParentID == parentNode.Id);
            if (parentNode != null)
            {
                parentNode.Childs = new List<ArticleMenuViewModel>();
            }

            foreach (var node in nodes)
            {
                ArticleMenuViewModel newNode = new ArticleMenuViewModel();
                newNode.Id = node.ID;
                newNode.Name = node.Name;
                newNode.ParentId = node.ParentID;

                if (parentNode == null)
                {
                    treeViewList.Add(newNode);
                }
                else
                {
                    parentNode.Childs.Add(newNode);
                }

                MakeTree(list, newNode);
            }
            return treeViewList;
        }
        public static List<ArticleViewModel> GetArticleUsingSearch(string searchValue, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var articles = (from article in db.Articles
                            join findArticle in db.ArticleSearch(searchValue, 10) on article.ID equals findArticle.ID
                            orderby findArticle.Rank ascending
                            select article);


            var searchResult = (from article in articles
                                join people in db.Peoples on article.User_AuthorID equals people.ID
                                select new ArticleViewModel
                                {
                                    Id = article.ID,
                                    ParentId = article.ParentID,
                                    Name = article.Name,
                                    Breadcrumbs = article.Breadcrumbs,
                                    AuthorName = people.FirstName + " " + people.LastName,
                                    Summery = article.Summery,
                                    ArticleTypeId = article.ArticleTypeID,
                                    UploadDate = CalendarService.ConvertToPersian(article.UploadDate).ToString("HH:mm yyyy/MM/dd"),
                                    HasChild = db.Articles.Any(el => el.ParentID == article.ID)
                                }).ToList();
            return searchResult;
        }
        private static MatchCollection SearchArtcileBodyUsingRegex(string input)
        {
            var regex = new Regex("\\[.*]\\(.*\\)", RegexOptions.IgnoreCase);
            var results = regex.Matches(input);
            return results;
        }
        public static List<string> GetAllArticlesKeyWord(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var kewWords = new List<string>();
            var q = db.Articles.Select(el => el.KeyWords);

            foreach (var item in q)
            {
                kewWords.AddRange(item.Split(','));
            }

            return kewWords;
        }
        public static List<BreadCrumbViewModel> GetBreadCrumbListOnArticleId(int articleId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = db.GetArticleAncestor(articleId);

            var result = q.Select(el => new BreadCrumbViewModel
            {
                Id = el.ID,
                Level = el.Level,
                Name = el.Name
            }).OrderBy(e => e.Level).ToList();
            return result;
        }
        public static void SaveArticleVisit(int articleId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string IPAddress;

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    IPAddress = addresses[0];
                }
            }

            IPAddress = context.Request.ServerVariables["REMOTE_ADDR"];

            try
            {
                var dbmodel = new CORE.UserViewHistory
                {
                    ArticleID = articleId,
                    IP = IPAddress,
                    LogDate = DateTime.Now
                };

                db.UserViewHistories.InsertOnSubmit(dbmodel);
                db.SubmitChanges();
                CoreDataContext.SetDbSessionInfo(db, userId);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public static MetaDataViewModel GetArticleMetaData(int articleId)
        {
            return new MetaDataViewModel
            {
                Title = "test article1 title",
                Description = "test article1 description"
            };
        }
    }
}
