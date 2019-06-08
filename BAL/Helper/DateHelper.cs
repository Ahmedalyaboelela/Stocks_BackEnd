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
    }
}
