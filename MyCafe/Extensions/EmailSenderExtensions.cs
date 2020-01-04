using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MyCafe.Services;
using MyCafe.Utility;

namespace MyCafe.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static Task SendStatusEmailAsync(this IEmailSender emailSender, string email, string status, string orderId)
        {
            var subject = "";
            var message = "";

            if(status==SD.StatusCancelled)
            {
                subject = "Order Cancelled";
                message = "<h4>Order Number: <strong>" + orderId + "</strong ></h4>" + "<p>Has Been Cancelled…….<p/>" + "<p>Thank You!<p/>" + "<p><strong> MyCafe Team </strong><p/>"; ;
            }
            if (status == SD.StatusSubmitted)
            {
                subject = "Order Placed Successfully";
                message = "<h4>Order Number: <strong>" + orderId + "</strong ></h4>" + "<p>Has Been Placed Successfully…….<p/>" + "<p>Thank You!<p/>" + "<p><strong> MyCafe Team </strong><p/>";
            }
            if (status == SD.StatusReady)
            {
                subject = "Order Ready";
                message = "<h4>Order Number: <strong>" + orderId + "</strong ></h4>" + "<p>Is Ready For Pickup......<p/>" + "<p>Thank You!<p/>" + "<p><strong> MyCafe Team </strong><p/>";
            }
            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
