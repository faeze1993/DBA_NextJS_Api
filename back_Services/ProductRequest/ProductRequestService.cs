using back_Models.ProductRequest;
using CORE;
using Models.MessageClass;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Services.ProductRequest
{
    public class ProductRequestService
    {
        public static MessageClass Save(ProductRequestViewModel model, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var dbModel = CreateModel(model);
            var mc = Insert(dbModel, ref db);
            return mc;
        }
        private static CORE.ProductRequest CreateModel(ProductRequestViewModel model)
        {
            var db = new CoreDataContext();

            var connection = db.Connection;
            connection.Open();
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
            var obj = cmd.ExecuteScalar();
            var anInt = (Int32)obj;

            var productRequest = new CORE.ProductRequest()
            {
                ID = model.Id ?? anInt,
                FullName = model.FullName,
                Mobile = model.Mobile,
                Product = model.Products,
                Description = model.Description,
                Date = DateTime.Now,
                IsReplied = false,
            };
            return productRequest;
        }

        public static MessageClass Insert(CORE.ProductRequest item, ref CoreDataContext db)
        {

            var ec = new MessageClass();

            db.ProductRequests.InsertOnSubmit(item);
            try
            {
                db.SubmitChanges();
                ec.Message = "نظر شما با موفقیت ثبت گردید";
                ec.Status = "success";
            }
            catch (Exception e)
            {
                ec.Message = "خطایی رخ داده!" + e.Message;
                ec.Status = "error";
            }
            return ec;
        }

        public static MessageClass Update(int id, int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var productRequest = db.ProductRequests.SingleOrDefault(el => el.ID == id);
            if (productRequest == null) return ec;
            productRequest.IsReplied = !productRequest.IsReplied;

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
        public static MessageClass Delete(int id, int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var productRequest = db.ProductRequests.Where(el => el.ID == id).Select(el => el).SingleOrDefault();
            db.ProductRequests.DeleteOnSubmit(productRequest);
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
        public static IQueryable<ProductRequestViewModel> GetProductRequestGridData(string searchValue,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var productRequests = db.ProductRequests.Select(el => el);

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var strSearchingValue = string.Format("%{0}%", searchValue.Trim().Replace(' ', '%'));
                productRequests = productRequests.Where(el => SqlMethods.Like(el.Product, strSearchingValue));
            }
            var q = (from productRequest in productRequests
                     select new ProductRequestViewModel
                     {
                         Id = productRequest.ID,
                         Description = productRequest.Description,
                         FullName = productRequest.FullName,
                         Products = productRequest.Product,
                         Mobile = productRequest.Mobile,
                         Date = productRequest.Date,
                         IsReplied = productRequest.IsReplied,
                         PersianDate = CalendarService.ConvertToPersian(productRequest.Date).ToString("HH:mm yyyy/MM/dd"),
                     }).OrderBy(el => el.Date);
            return q;
        }
    }
}
