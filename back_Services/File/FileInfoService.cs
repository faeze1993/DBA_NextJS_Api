using back_Models.Article;
using back_Models.File;
using CORE;
using Models.MessageClass;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace back_Services.File
{
    public class FileInfoService
    {
        public static MessageClass SaveSingleFile(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var file = HttpContext.Current.Request.Files;
            var directoryName = HttpContext.Current.Request.Params["directoryName"];


            var newId = Guid.NewGuid();

            var byteArray = StreamToByteArray(file[0].InputStream);
            var fileName = string.Format("{0}_{1}", newId, file[0].FileName);
            db.SaveFiles(newId, fileName, byteArray, directoryName);



            try
            {
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";
                mc.Data = newId;

            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!" + e.Message;
                mc.Status = "error";
            }

            return mc;
        }
        public static MessageClass SaveFiles(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var files = HttpContext.Current.Request.Files;
            var directoryName = HttpContext.Current.Request["directoryName"];


            var listfile = new List<FileInfoViewModel>();
            for (var i = 0; i < files.Count; i++)
            {
                if (files[i] != null && files[i].ContentLength > 0)
                {
                    var newId = Guid.NewGuid();
                    listfile.Add(new FileInfoViewModel()
                    {
                        stream_id = newId,
                        file_stream = StreamToByteArray(files[i].InputStream),
                        name = files[i].FileName,
                    });
                }
            }


            foreach (var item in listfile)
            {
                db.SaveFiles(item.stream_id, item.name, item.file_stream, directoryName);
            }



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
        public static byte[] StreamToByteArray(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
        public static MessageClass GetRelativeUrl(Guid streamId)
        {
            var db = new CoreDataContext();
            var mc = new MessageClass();
            var reUrl = db.FilesViews.Where(el => el.stream_id == streamId).Select(el => el.directory).SingleOrDefault();

            var abUrl = reUrl.Replace(@"\", "/");

            mc.Data = abUrl;
            return mc;
        }
        public static Guid SaveImage(HttpPostedFile file, string directoryName,int userId, ref CoreDataContext db)
        {
            //var db = new CoreDataContext();
            var mc = new MessageClass();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var newId = Guid.NewGuid();

            var byteArray = StreamToByteArray(file.InputStream);
            var fileName = string.Format("{0}_{1}", newId, file.FileName);
            db.SaveFiles(newId, fileName, byteArray, directoryName);

            //try
            //{
            //    db.SubmitChanges();
            //}
            //catch (Exception e)
            //{
            //    mc.Message = "خطایی رخ داده!" + e.Message;
            //    mc.Status = "error";
            //}

           
            return newId;
        }
        public static List<FileManagementViewModel> GetAllChildNodeOfDirectory(Guid? streamId, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = db.GetChiledNode(streamId).Select(el => new FileManagementViewModel {
                Stream_id = el.stream_id,
                Name = el.name,
                Directory = el.directory.Replace(@"\", "/"),
                Size = el.cached_file_size != null ? Math.Round((double)el.cached_file_size / 1048576,2): 0 ,
                IsDirectory = el.is_directory,
                CreatationDate = el.creation_time,
                PersianCreatationDate = CalendarService.ConvertToPersian((DateTime)el.creation_time).ToString("HH:mm yyyy/MM/dd"),
            }).ToList();

            return q;
        }
        public static List<FileManagementViewModel> GetFilesBySerach(string searchValue,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            if (string.IsNullOrEmpty(searchValue))
                return GetAllChildNodeOfDirectory(null,userId);

            var strSearchingValue = string.Format("%{0}%", searchValue.Trim().Replace(' ', '%'));

            var files = (from fileView in db.FilesViews
                         orderby fileView.creation_time descending
                         where SqlMethods.Like(fileView.name, strSearchingValue)
                            select fileView);
            var q = files.Select(el => new FileManagementViewModel
            {
                Stream_id = el.stream_id,
                Name = el.name,
                Directory = el.directory.Replace(@"\", "/"),
                //Size = el.cached_file_size != 0 ? Math.Round((double)el.cached_file_size / 1048576, 2) : 0,
                IsDirectory = el.is_directory,
                CreatationDate = DateTimeOffset.Parse(el.creation_time.ToString()).DateTime,
                PersianCreatationDate = CalendarService.ConvertToPersian(DateTimeOffset.Parse(el.creation_time.ToString()).DateTime).ToString("HH:mm yyyy/MM/dd"),
            }).ToList();

            return q;
        }
        public static MessageClass SaveFileOrFolderDirectly(int userId)
        {
           
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();
            var files = HttpContext.Current.Request.Files;
            var parentFolderPath = HttpContext.Current.Request.Params["ParentFolderPath"];
            var filderName = HttpContext.Current.Request.Params["Name"];
            bool.TryParse(HttpContext.Current.Request.Params["IsDirectory"], out bool isDirectory);

            string startupPath3 = Directory.GetCurrentDirectory();
            //var parentFolderPathWithoutSlash = parentFolderPath != string.Empty ? parentFolderPath.Substring(1) : string.Empty;

            if (isDirectory)
            {
                try
                {
                    string startupPath1 = System.IO.Directory.GetCurrentDirectory();
                    //string startupPath2 = Environment.CurrentDirectory;
                    //string startupPath3 = AppDomain.CurrentDomain.BaseDirectory;

                    var dir = (parentFolderPath == string.Empty || parentFolderPath == "undefined")
                           ? Path.Combine(startupPath3.Replace(@"\", "/"), HttpContext.Current.Server.MapPath("~/DBA_DIRECTORY/"), filderName)
                           : Path.Combine(startupPath3.Replace(@"\","/"), HttpContext.Current.Server.MapPath(parentFolderPath), filderName);

                    //var dir = parentFolderPath == string.Empty
                    //       ? Path.Combine(HttpContext.Current.Request.MapPath("http://localhost/pic/"), filderName)
                    //       : Path.Combine(parentFolderPath, filderName);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        mc.Message = "فولدر با نام مورد نظر با موفقیت ایجاد گردید ";
                        mc.Status = "success";
                    }
                    else
                    {
                        mc.Message = "فولد با نام مورد نظر قبلا ایجاد گردیده است ";
                        mc.Status = "error";
                    }
                }
                catch (Exception e)
                {

                    mc.Message = $"در فرآیند ایجاد فولدر مورد نظر خطایی رخ داده است. لطفا مجددا تلاش نمایید ";
                    mc.Status = "error";
                }
              
            }
            else
            {
                try
                {
                    if (files.Count > 0)
                    {
                        var file = files[0];
                        var fileName = file.FileName;
                        if (file != null && file.ContentLength > 0)
                        {
                            var path = (parentFolderPath == string.Empty || parentFolderPath == "undefined")
                                 ? Path.Combine(startupPath3.Replace(@"\", "/"), HttpContext.Current.Server.MapPath("~/DBA_DIRECTORY/"), fileName)
                                 : Path.Combine(startupPath3.Replace(@"\", "/"), HttpContext.Current.Server.MapPath(parentFolderPath), fileName);


                            file.SaveAs(path);
                            mc.Message = "فایل  مورد نظر با موفقیت ذخیره گردید ";
                            mc.Status = "success";
                            var dbFile = db.FilesViews.Where(el => el.directory == path).Select(el => el);
                        }
                    }

                }
                catch (Exception e)
                {

                    mc.Message = "در فرآیند ذخیره فایل خطایی رخ داده است. لطفا مجددا تلاش نمایید " + e;
                    mc.Status = "error";
                }
               
            }



            return mc;
        }
        public static List<FileBreadCrumbViewModel> GetBreadCrumbListOnStreamId(Guid? streamId,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = db.GetFiesAncestor(streamId).ToList();

            var result = q.Select(el => new FileBreadCrumbViewModel
            {
                StreamId = el.stream_id,
                Level = el.Level,
                Name = el.name
            }).OrderBy(e => e.Level).ToList();
            return result;
        }
        public static MessageClass DeleteFileOrFolder(Guid? streamId,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var isInUserFile = db.UserFiles.Any(el => el.FilesID == streamId);
            if (isInUserFile)
            {
                var dbUserFile = db.UserFiles.SingleOrDefault(el => el.FilesID == streamId);
                db.UserFiles.DeleteOnSubmit(dbUserFile);
                db.SubmitChanges();
            }
            try
            {
                var sql = "DELETE FROM [dbo].[Files] WHERE stream_id = {0}";
                db.ExecuteCommand(sql, streamId);

                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";

            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!" ;
                mc.Status = "error";
            }


            return mc;
        }
        public static MessageClass SaveUserFile(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var file = HttpContext.Current.Request.Files;
            var directoryName = HttpContext.Current.Request.Params["directoryName"];


            var newId = Guid.NewGuid();

            var byteArray = StreamToByteArray(file[0].InputStream);
            var fileName = string.Format("{0}", file[0].FileName);
            db.SaveFiles(newId, fileName, byteArray, directoryName);

            var connection = db.Connection;
            connection.Open();
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
            var obj = cmd.ExecuteScalar();
            var anInt = (Int32)obj;

            var userFile = new CORE.UserFile
            {
                ID = anInt,
                FilesID = newId,
                UserID = userId
            };

            db.UserFiles.InsertOnSubmit(userFile);

            try
            {
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";
                mc.Data = newId;

            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!" + e.Message;
                mc.Status = "error";
            }

            return mc;
        }
        public static List<UserFilesViewModel> GetUserfileList(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var q = (from filesView in db.FilesViews
                     join userFile in db.UserFiles on filesView.stream_id equals userFile.FilesID
                     where userFile.UserID == userId
                     select new UserFilesViewModel {
                         Id = userFile.ID,
                         StreamId = filesView.stream_id,
                         Name = filesView.name,
                         Directory = filesView.directory.Replace(@"\", "/"),
                         CreatationDate = DateTimeOffset.Parse(filesView.creation_time.ToString()).DateTime ,
                         PersianCreatationDate = CalendarService.ConvertToPersian(DateTimeOffset.Parse(filesView.creation_time.ToString()).DateTime).ToString("HH:mm yyyy/MM/dd"),
                     }).ToList();

           
            return q;
        }
    }
}
