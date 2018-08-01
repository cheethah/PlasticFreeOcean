using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PlasticFreeOcean.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _env;

        public EmailService(IConfiguration configuration,IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
  
        public string CreateEmailBodyKonfirmasi(string userName,string link)  
      
        {  
            string body = string.Empty;   
            var webRoot = _env.WebRootPath;
            var file = Path.Combine(webRoot, "TemplateKonfirmasiEmail.html");
            using(StreamReader reader = new StreamReader(file))  
            {  
                body = reader.ReadToEnd(); 
            }  
      
            body = body.Replace("{UserName}", userName); 
      
            body = body.Replace("{link}", link);  
      
            return body;  
      
        }  
        public async Task SendEmail(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = _configuration["SMTPdata:Email"],
                    Password = _configuration["SMTPdata:Password"]
                };

                client.Credentials = credential;
                client.Host = _configuration["SMTPdata:Host"];
                client.Port = int.Parse(_configuration["SMTPdata:Port"]);
                client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress(_configuration["SMTPdata:Email"]);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;
                    client.Send(emailMessage);
                }
            }
            await Task.CompletedTask;
        }

    }
}
