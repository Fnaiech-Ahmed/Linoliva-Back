using System.Net;
using System.Net.Mail;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(ContactMessage message)
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"]
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = $"[Contact depuis site web] {message.Sujet}",
                IsBodyHtml = true, // Indique que le corps est en HTML
                Body = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f9;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                width: 100%;
                                max-width: 600px;
                                margin: 0 auto;
                                background-color: #fff;
                                padding: 30px;
                                border-radius: 8px;
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                            }}
                            h2 {{
                                color: #2980b9;
                                font-size: 26px;
                                margin-bottom: 15px;
                            }}
                            p {{
                                font-size: 16px;
                                color: #555;
                                line-height: 1.8;
                            }}
                            .message-body {{
                                background-color: #ecf0f1;
                                padding: 20px;
                                border-radius: 6px;
                                margin-top: 20px;
                                font-size: 14px;
                                color: #2c3e50;
                            }}
                            .footer {{
                                text-align: center;
                                font-size: 14px;
                                color: #888;
                                margin-top: 30px;
                            }}
                            .footer a {{
                                color: #2980b9;
                                text-decoration: none;
                            }}
                            .header {{
                                background-color: #2980b9;
                                color: #fff;
                                padding: 15px;
                                border-radius: 8px 8px 0 0;
                                text-align: center;
                            }}
                            .header h1 {{
                                margin: 0;
                                font-size: 28px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Contact Form Submission</h1>
                            </div>
                            <h2>Message de {message.Nom} ({message.Email})</h2>
                            <p><strong>Sujet:</strong> {message.Sujet}</p>
                            <div class='message-body'>
                                <p><strong>Message:</strong></p>
                                <p>{message.Message}</p>
                            </div>
                            <div class='footer'>
                                <p>Merci pour votre message. Nous vous répondrons dans les plus brefs délais.</p>
                                <p>Pour plus d'informations, visitez <a href='https://softwareengineerconsultant.tech/'>notre site web</a>.</p>
                            </div>
                        </div>
                    </body>
                    </html>"
            };

            mailMessage.To.Add(_configuration["Smtp:To"]);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
