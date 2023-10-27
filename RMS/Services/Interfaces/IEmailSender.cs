using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.Services.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string senderEmail, string senderName, string receieverEmail, string receieverName, string subject, string emailBody);
    }
}
