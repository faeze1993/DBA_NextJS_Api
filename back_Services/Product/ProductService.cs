using back_Models.Common;
using back_Models.Product;
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

namespace back_Services
{
    public class ProductService
    {
        public static MessageClass Insert(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var title = HttpContext.Current.Request.Params["Title"];
            var linkUrl = HttpContext.Current.Request.Params["LinkUrl"];
            var description = HttpContext.Current.Request.Params["Description"];
            bool.TryParse(HttpContext.Current.Request.Params["IsActive"], out var isActive);
            var ImageFile = HttpContext.Current.Request.Files;

            var imageId = FileInfoService.SaveImage(ImageFile[0], "product", userId, ref db);

            CORE.Product product = new CORE.Product
            {
                Title = title,
                Description = description,
                IsActive = isActive,
                LinkUrl = linkUrl,
                Files_ImageID = imageId
            };

            db.Products.InsertOnSubmit(product);

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
        public static MessageClass Edit(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            int.TryParse(HttpContext.Current.Request.Params["Id"], out var id);
            bool.TryParse(HttpContext.Current.Request.Params["IsActive"], out var isActive);
            var title = HttpContext.Current.Request.Params["Title"];
            var description = HttpContext.Current.Request.Params["Description"];
            var linkUrl = HttpContext.Current.Request.Params["LinkUrl"];
            var imageFile = HttpContext.Current.Request.Files["ImageFile"];
            Guid.TryParse(HttpContext.Current.Request.Params["ImageId"], out var imageId);

            var dbProduct = db.Products.SingleOrDefault(el => el.ID == id);


            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var newImageId = FileInfoService.SaveImage(imageFile, "product", userId, ref db);
                dbProduct.Files_ImageID = newImageId;

                var oldImage = db.FilesViews.Where(el => el.stream_id == imageId).Select(el => el).SingleOrDefault();
                if (oldImage != null)
                    db.FilesViews.DeleteOnSubmit(oldImage);
            }

            dbProduct.Title = title;
            dbProduct.Description = description;
            dbProduct.IsActive = isActive;
            dbProduct.LinkUrl = linkUrl;

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

        public static MessageClass Delete(int Id, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var product = db.Products.SingleOrDefault(el => el.ID == Id);

            if (product != null)
            {
                db.Products.DeleteOnSubmit(product);

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
            db.ExecuteCommand(sql, product.Files_ImageID);
            return mc;
        }

        public static ProductViewModel GetDetail(int Id, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from product in db.Products
                     join fileView in db.FilesViews on product.Files_ImageID equals fileView.stream_id
                     where product.ID == Id
                     select new ProductViewModel()
                     {
                         Id = product.ID,
                         Title = product.Title,
                         IsActive = product.IsActive,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),/* "http://localhost/pic/" + fileView.directory.Replace("\\DIRECTORY_DBA\\", "").Replace(@"\", "/"),*/
                         FileInfo_ImageId = product.Files_ImageID,
                         LinkUrl = product.LinkUrl
                     }).SingleOrDefault();

            return q;
        }
        public static List<ProductViewModel> GetAllProductsForPanel(string searchValue,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var products = db.Products.Select(el => el);

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var strSearchValue = string.Format("%{0}%", searchValue).Replace(" ", "%");
                products = products.Where(el => SqlMethods.Like(el.Title, strSearchValue));
            }
            var q = (from product in products
                     join fileView in db.FilesViews on product.Files_ImageID equals fileView.stream_id
                     select new ProductViewModel()
                     {
                         Id = product.ID,
                         Title = product.Title,
                         Description = product.Description,
                         IsActive = product.IsActive,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),/*"http://localhost/pic/" + fileView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/"),*/
                         FileInfo_ImageId = product.Files_ImageID,
                         LinkUrl = product.LinkUrl
                     }).ToList();
            return q;
        }

        public static List<ProductViewModel> GetAllProducts(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from product in db.Products
                     join fileView in db.FilesViews on product.Files_ImageID equals fileView.stream_id
                     where product.IsActive 
                     select new ProductViewModel()
                     {
                         Id = product.ID,
                         Title = product.Title,
                         Description = product.Description,
                         IsActive = product.IsActive,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),/*"http://localhost/pic/" + fileView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/"),*/
                         FileInfo_ImageId = product.Files_ImageID,
                         LinkUrl = product.LinkUrl
                     }).ToList();
            return q;
        }

        public static List<ValueLabelViewModel> GetProductList()
        {
            var db = new CoreDataContext();
            var q = db.Products.Where(el => el.IsActive).Select(el => new ValueLabelViewModel { 
                value = el.ID,
                label = el.Title
            }).ToList();

            return q;
        }
    }
}
