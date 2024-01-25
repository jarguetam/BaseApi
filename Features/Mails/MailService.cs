using System;
using System.Collections.Generic;
using BaseApi.WebApi.Features.Common.Dto;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Linq;
using BaseApi.WebApi.Features.Mails.Dto;

namespace BaseApi.WebApi.Features.Mails
{
    public class MailService
    {
        private readonly IConfiguration _emailConfig;
        public MailService(IConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        /// <summary>
        /// Performs a custom action: ...
        /// </summary>
        /// <param name="x">
        /// An integer representing the ...
        /// </param>
        /// <param name="y">
        /// A boolean representing the ...
        /// </param>
        ///
        public void SendEmailHtm(List<string> Emails, string Title, string BodyHtml)
        {
            try
            {
                //para leer un archivo a byte
                //byte[] PdfInbytes = System.IO.File.ReadAllBytes(path);

                var emailMessage = new MimeMessage();
                var address = InternetAddress.Parse(_emailConfig["EmailConfiguration:From"]);
                emailMessage.From.Add(address);
                emailMessage.To.AddRange(Emails.Select(x=>  InternetAddress.Parse(x)));
                emailMessage.Subject = Title;
                var bodyBuilderS = new BodyBuilder
                {
                    HtmlBody = BodyHtml
                };

                emailMessage.Body = bodyBuilderS.ToMessageBody();



                Send(emailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SendEmailHtmAttachments(List<string> Emails, string Title, string BodyHtml, List<AttachmentEmail> Attachments)
        {
            try
            {
                var emailMessage = new MimeMessage();
                var address = InternetAddress.Parse(_emailConfig["EmailConfiguration:From"]);
                address.Name = "BTD GROUP";
                emailMessage.From.Add(address);
                emailMessage.To.AddRange(Emails.Select(x => InternetAddress.Parse(x)));
                emailMessage.Subject = Title;
                var bodyBuilderS = new BodyBuilder
                {
                    HtmlBody = BodyHtml
                };
                Attachments.ForEach((item) =>
                {
                bodyBuilderS.Attachments.Add($"{item.Title}", item.File, ContentType.Parse(item.Type));/// "Application/pdf"));
                });
                emailMessage.Body = bodyBuilderS.ToMessageBody();
                Send(emailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    string smtpServer = _emailConfig["EmailConfiguration:SmtpServer"];
                    int port = int.Parse(_emailConfig["EmailConfiguration:Port"]);
                    client.Connect(smtpServer, port, SecureSocketOptions.StartTls);
                    //client.Connect(smtpServer, port);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig["EmailConfiguration:UserName"], _emailConfig["EmailConfiguration:Password"]);

                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
