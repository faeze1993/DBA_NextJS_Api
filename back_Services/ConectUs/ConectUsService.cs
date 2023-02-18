using back_Models.ConectUs;
using CORE;
using Models.MessageClass;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Services.ConectUs
{
    public class ConectUsService
    {
        public static MessageClass Save(ConectUsViewModel model, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var dbModel = CreateModel(model);
            var mc = Insert(dbModel, ref db);
            return mc;
        }
        private static CORE.ConectUs CreateModel(ConectUsViewModel model)
        {
            var db = new CoreDataContext();

            var connection = db.Connection;
            connection.Open();
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
            var obj = cmd.ExecuteScalar();
            var anInt = (Int32)obj;

            var conectUs = new CORE.ConectUs()
            {
                ID = model.Id ?? anInt,
                FullName = model.FullName,
                Email = model.Email,
                Subject = model.Subject,
                Description = model.Description,
                Date = DateTime.Now,
                IsReplied = false,
            };
            return conectUs;
        }
        public static MessageClass Insert(CORE.ConectUs item, ref CoreDataContext db)
        {

            var ec = new MessageClass();

            db.ConectUs.InsertOnSubmit(item);
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
        public static MessageClass Update(int id,int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var contentUs = db.ConectUs.SingleOrDefault(el => el.ID == id);
            if (contentUs == null) return ec;
            contentUs.IsReplied = !contentUs.IsReplied;
           
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
            var comment = db.ConectUs.Where(el => el.ID == id).Select(el => el).SingleOrDefault();
            db.ConectUs.DeleteOnSubmit(comment);
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
        public static IQueryable<ConectUsViewModel> GetConectUsGridData(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from conectUs in db.ConectUs
                     select new ConectUsViewModel
                     {
                         Id = conectUs.ID,
                         Description = conectUs.Description,
                         FullName = conectUs.FullName,
                         Subject = conectUs.Subject,
                         Email = conectUs.Email,
                         Date = conectUs.Date,
                         IsReplied = conectUs.IsReplied,
                         PersianDate = CalendarService.ConvertToPersian(conectUs.Date).ToString("HH:mm yyyy/MM/dd"),
                     }).OrderBy(el => el.Date);
            return q;
        }
    }
}
