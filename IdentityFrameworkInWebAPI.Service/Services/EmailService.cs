﻿using IdentityFrameworkInWebAPI.Service.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityFrameworkInWebAPI.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig)=> _emailConfig = emailConfig;

        public void SendEmail(Message message)
        {
           var emailMessage=CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message) { 
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body= new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage) {
            using var client = new SmtpClient();

            try
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch
            {
                //log an erroe message or thow an exeveption or both
                throw;
            }
            finally {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
