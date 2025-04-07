using System.Net.Mail;
using System.Net;

namespace Company.MVC.PL.Helpers
{
    public static class EmailSettings
    {
        public static bool SendEmail (Email email)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com.\r\n", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("hassanmenna497@gmail.com\r\n", "utvkcnputnbcsgcs");
                client.Send("hassanmenna497@gmail.com", email.To, email.Subject, email.Body);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
