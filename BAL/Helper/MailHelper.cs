using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace BAL.Helper
{
   public static class MailHelper
    {
        //General Config For Sending Mail
        public static bool SendMail(string to, string subject, string answer)
        {

            var from = "";
            MailMessage mail = new MailMessage(from, to);
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Credentials = new System.Net.NetworkCredential(from, ""),
                EnableSsl = true
            };
            mail.Subject = subject;
            mail.Body = "" + answer + "";
            mail.IsBodyHtml = true;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
