using RMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RMS.Services
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(string senderEmail, string senderName, string receieverEmail, string receieverName, string subject, string emailBody)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    string from = "n.nwabuike2003@gmail.com";

                    message.To.Add(new MailAddress(receieverEmail, receieverName));
                    message.From = new MailAddress(from, senderName);
                    //message.CC.Add(new MailAddress("cc@email.com", "CC Name"));
                    //message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));
                    message.Subject = subject;
                    message.Body = emailBody;
                    message.IsBodyHtml = true;


                    using (var client = new System.Net.Mail.SmtpClient("smtp.gmail.com"))
                    {
                        client.Port = 587;
                        client.UseDefaultCredentials = true;
                        client.Credentials = new NetworkCredential("n.nwabuike2003@gmail.com", "Noble2003");
                        client.EnableSsl = true;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) => true;
                        client.Send(message);
                    }
                }

            }
            catch (System.Exception ex)
            {
                var error = ex;
            }
        }
    }
}
