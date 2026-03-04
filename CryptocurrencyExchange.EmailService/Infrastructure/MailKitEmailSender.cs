using CryptocurrencyExchange.EmailService.Interfaces;
using CryptocurrencyExchange.EmailService.Options;
using CryptocurrencyExchange.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CryptocurrencyExchange.EmailService.Infrastructure
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(IOptions<SmtpOptions> options, ILogger<MailKitEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            try
            {
                using var client = new SmtpClient();
                var socketOptions = _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                await client.ConnectAsync(_options.Host, _options.Port, socketOptions, ct);
                await client.AuthenticateAsync(_options.Username, _options.Password, ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);

                _logger.LogInformation("Email sent to {Recipient}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", to);
                throw new EmailSendingFailedException(to, ex);
            }
        }
    }
}
