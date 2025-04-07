using System.Net.Mail;
using Company.MVC.PL.Settings;
using MailKit.Net.Smtp;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Company.MVC.PL.Helpers
{
    public class MailService(IOptions<Mailsettings> _options) : IMailService
    {       
        public void sendEmail(Email email)
        {
            var mail = new MimeMessage();

            mail.Subject = email.Subject;
            mail.From.Add(new MailboxAddress( _options.Value.DisplayName, _options.Value.Email));
            mail.To.Add(MailboxAddress.Parse(email.To)); 

            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body = builder.ToMessageBody();


            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_options.Value.Host, _options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Value.Email, _options.Value.password);


            smtp.Send(mail);
        }
    }
}
