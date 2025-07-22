using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration _configuration)
        {
            _configuration = _configuration;
        }

        /* public async Task SendEmailAsync(string email, string subject, string htmlMessage)
         {
             var apiKey = _config["SendGrid:ApiKey"];
             Console.WriteLine("SendGrid API Key: " + (string.IsNullOrEmpty(apiKey) ? "NOT FOUND" : "FOUND"));

             var client = new SendGridClient(apiKey);
             var from = new EmailAddress("nikolovskat95@gmail.com", "Health Condition Forecast");
             var to = new EmailAddress(email);
             var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);
             var response=await client.SendEmailAsync(msg);
             Console.WriteLine($"SendGrid status: {response.StatusCode}");
             string responseBody = await response.Body.ReadAsStringAsync();
             Console.WriteLine($"SendGrid response body: {responseBody}");
         }*/
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            //emailToSend.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
            emailToSend.From.Add(new MailboxAddress("Health Condition Forecast", "nikolovskat95@gmail.com"));

            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("nikolovskat95@gmail.com", "akmh wety wzjb plun");
            await smtp.SendAsync(emailToSend);
            await smtp.DisconnectAsync(true);

        
    }
    }


