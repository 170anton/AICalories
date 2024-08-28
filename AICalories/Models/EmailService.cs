using System;
using System.Net;
using System.Net.Mail;

namespace AICalories.Models
{
    public static class EmailService
    {
        public static async Task<bool> SendEmailAsync(string email, string question, string version)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Use 465 if SSL is required
                    Credentials = new NetworkCredential("appaicalories@gmail.com", "iiaq eqsl jubp iedt"),
                    EnableSsl = true,
                    
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("appaicalories@gmail.com"),
                    Subject = "Support Question",
                    Body = $"Email: {email}\n\nQuestion:\n{question}\n{version}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add("appaicalories@gmail.com"); // Sending to the same email address

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}

