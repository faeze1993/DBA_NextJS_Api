using back_Models.Slider;
using back_Services.File;
using CORE;
using Models.MessageClass;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace back_Services.Slider
{
    public class SliderService
    {
        public static MessageClass InsertSlider(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var name = HttpContext.Current.Request.Params["Name"];
            var linkUrl = HttpContext.Current.Request.Params["LinkUrl"];
            bool.TryParse(HttpContext.Current.Request.Params["IsActive"], out var isActive);
            var ImageFile = HttpContext.Current.Request.Files;

            var imageId = FileInfoService.SaveImage(ImageFile[0], "slider",userId, ref db);

            CORE.Slider slider = new CORE.Slider
            {
                Name = name,
                IsActive = isActive,
                LinkUrl = linkUrl,
                Files_ImageID = imageId
            };

            db.Sliders.InsertOnSubmit(slider);

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
        public static MessageClass EditSlider(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            int.TryParse(HttpContext.Current.Request.Params["Id"], out var id);
            bool.TryParse(HttpContext.Current.Request.Params["IsActive"], out var isActive);
            var name = HttpContext.Current.Request.Params["Name"];
            var linkUrl = HttpContext.Current.Request.Params["LinkUrl"];
            var imageFile = HttpContext.Current.Request.Files["ImageFile"];
            Guid.TryParse(HttpContext.Current.Request.Params["ImageId"], out var imageId);

            var dbSlider = db.Sliders.SingleOrDefault(el => el.ID == id);


            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var newImageId = FileInfoService.SaveImage(imageFile, "slider",userId, ref db);
                dbSlider.Files_ImageID = newImageId;

                var oldImage = db.FilesViews.Where(el => el.stream_id == imageId).Select(el => el).SingleOrDefault();
                if (oldImage != null)
                    db.FilesViews.DeleteOnSubmit(oldImage);
            }

            dbSlider.Name = name;
            dbSlider.IsActive = isActive;
            dbSlider.LinkUrl = linkUrl;

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

        public static MessageClass DeleteSlider(int Id, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var slider = db.Sliders.SingleOrDefault(el => el.ID == Id);

            if (slider != null)
            {
                db.Sliders.DeleteOnSubmit(slider);
                
            }
            else
            {
                mc.Message = "مشکلی پیش امده است";
                mc.Status = "error";
                return mc;
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
                return mc;
            }

            var sql = "DELETE FROM [dbo].[Files] WHERE stream_id = {0}";
            db.ExecuteCommand(sql, slider.Files_ImageID);
            return mc;
        }

        public static SliderViewModel GetDetailSlider(int Id,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from slider in db.Sliders
                     join fileView in db.FilesViews on slider.Files_ImageID equals fileView.stream_id
                     where slider.ID == Id
                     select new SliderViewModel()
                     {
                         Id = slider.ID,
                         Name = slider.Name,
                         IsActive = slider.IsActive,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),/* "http://localhost/pic/" + fileView.directory.Replace("\\DIRECTORY_DBA\\", "").Replace(@"\", "/"),*/
                         ImageId = slider.Files_ImageID,
                         LinkUrl = slider.LinkUrl
                     }).SingleOrDefault();

            return q;
        }
        public static List<SliderViewModel> GetAllSliders(string searchText,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var sliders = db.Sliders.Select(el => el);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var strSearchingValue = string.Format("%{0}%", searchText.Trim().Replace(' ', '%'));
                sliders = sliders.Where(el => SqlMethods.Like(el.Name, strSearchingValue));
            }
            var q = (from slider in sliders
                     join fileView in db.FilesViews on slider.Files_ImageID equals fileView.stream_id
                     select new SliderViewModel()
                     {
                         Id = slider.ID,
                         Name = slider.Name,
                         IsActive = slider.IsActive,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),/*"http://localhost/pic/" + fileView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/"),*/
                         ImageId = slider.Files_ImageID,
                         LinkUrl = slider.LinkUrl
                     }).ToList();
            return q;
        }
    }
}
