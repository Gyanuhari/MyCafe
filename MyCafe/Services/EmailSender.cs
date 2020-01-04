using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyCafe.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            
            SmtpClient smtpClient = new SmtpClient()
            {
                Host="smtp.gmail.com",
                Port=587,
                UseDefaultCredentials=false,
                Credentials=new NetworkCredential("mycafemycafe88@gmail.com","Mycafe8Mycafe"),
                EnableSsl=true,
            };
            MailMessage mailMessage = new MailMessage("mycafemycafe88@gmail.com", email)  //first is from and second is to
            {
                Subject=subject,
                Body=message,
                IsBodyHtml=true
            };
            smtpClient.Send(mailMessage);

            return Task.CompletedTask;
        }
    }
}
