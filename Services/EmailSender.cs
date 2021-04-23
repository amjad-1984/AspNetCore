using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jolia.AspNetCore.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Jolia.Core.Features.Email.SendEmail(email, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
