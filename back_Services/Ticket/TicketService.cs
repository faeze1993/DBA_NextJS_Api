using back_Models.Ticket;
using CORE;
using Models.MessageClass;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Services.Ticket
{
    public class TicketService
    {
        public static MessageClass Save(TicketViewModel model,int userId)
        {
           
            var dbModel = CreateModel(model, userId);
            var mc = model.Id == null || model.Id == 0 ? Insert(dbModel, userId): Update(dbModel, userId);
            return mc;
        }
        private static CORE.Ticket CreateModel(TicketViewModel model, int userId)
        {
            var db = new CoreDataContext();
            
            var connection = db.Connection;
            connection.Open();
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR [dbo].[NewIntegerID]";
            var obj = cmd.ExecuteScalar();
            var anInt = (Int32)obj;

            var ticket = new CORE.Ticket()
            {
                ID = model.Id ?? anInt,
                UserId = userId,
                Subject = model.Subject,
                Description = model.Description,
                Date = DateTime.Now,
                IsReplied = false,
            };
            return ticket;
        }
        public static MessageClass Insert(CORE.Ticket item,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var ec = new MessageClass();

            db.Tickets.InsertOnSubmit(item);
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
        public static MessageClass ToggleIsReplied(int id)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();

            var ticket = db.Tickets.SingleOrDefault(el => el.ID == id);
            if (ticket == null) return ec;
            ticket.IsReplied = !ticket.IsReplied;

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
        public static MessageClass Update(CORE.Ticket item, int userId)
        {
            var ec = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var ticket = db.Tickets.SingleOrDefault(el => el.ID == item.ID);
            if (ticket == null) return ec;
            ticket.Subject = item.Subject;
            ticket.Description = item.Description;


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
            var comment = db.Tickets.Where(el => el.ID == id).Select(el => el).SingleOrDefault();
            db.Tickets.DeleteOnSubmit(comment);
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
        public static List<TicketViewModel> GetTicketGridData(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from ticket in db.Tickets
                     where ticket.UserId == userId
                     select new TicketViewModel
                     {
                         Id = ticket.ID,
                         Description = ticket.Description,
                         Subject = ticket.Subject,
                         Date = ticket.Date,
                         IsReplied = ticket.IsReplied,
                         PersianDate = CalendarService.ConvertToPersian(ticket.Date).ToString("HH:mm yyyy/MM/dd"),
                     }).OrderBy(el => el.Date).ToList();
            return q;
        }
        public static List<TicketViewModel> GetTicketGridDataForPanel(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from ticket in db.Tickets
                     join user in db.Users on ticket.UserId equals user.ID
                     select new TicketViewModel
                     {
                         Id = ticket.ID,
                         UserName = user.Username,
                         Description = ticket.Description,
                         Subject = ticket.Subject,
                         Date = ticket.Date,
                         IsReplied = ticket.IsReplied,
                         PersianDate = CalendarService.ConvertToPersian(ticket.Date).ToString("HH:mm yyyy/MM/dd"),
                     }).OrderBy(el => el.Date).ToList();
            return q;
        }
    }
}
