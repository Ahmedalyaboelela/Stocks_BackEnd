using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BAL.Helper
{
    public static class DateHelper
    {
        // Convert Georgerian Date To Um Alqura Date
        public static string GetHijriDate(DateTime? GDate)
        {
            var cultureInfo = new CultureInfo("ar-SA");
            cultureInfo.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            return String.Format(cultureInfo, "{0:dd/MM/yyyy}", GDate);
        }

        // Change Date Format To Work With Reports
        public static DateTime ChangeDateFormat(string FromDate)
        {
            DateTime date = DateTime.Parse(FromDate);
            string fdate = date.ToString("yyyy-dd-MM");
            return Convert.ToDateTime(fdate);
        }
    }
}
