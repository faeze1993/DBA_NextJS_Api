using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Services.Utility
{
    public class CalendarService
    {
        public static DateTime ConvertToGregorian(DateTime obj)
        {
            DateTime dt = new DateTime(obj.Year, obj.Month, obj.Day, obj.Hour, obj.Minute, obj.Second, new PersianCalendar());
            return dt;
        }
        public static DateTime ConvertToPersian(DateTime obj)
        {
            try
            {
                var persian = new PersianCalendar();
                var year = persian.GetYear(obj);
                var month = persian.GetMonth(obj);
                var day = persian.GetDayOfMonth(obj);
                var hour = persian.GetHour(obj);
                var minute = persian.GetMinute(obj);
                var second = persian.GetSecond(obj);
                DateTime persiandate = new DateTime(year, month, day, hour, minute, second);
                return persiandate;
            }
            catch (Exception e)
            {

                throw e;
            }
          
        }
    }
}