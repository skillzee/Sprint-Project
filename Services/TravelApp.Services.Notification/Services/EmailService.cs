using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace TravelApp.Services.Notification.Services;

/// <summary>
/// Sends transactional HTML emails via SMTP using MailKit.
/// SMTP settings (host, port, credentials, sender) are read from application configuration.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="EmailService"/>.
    /// </summary>
    /// <param name="config">The application configuration for reading SMTP settings.</param>
    /// <param name="logger">The logger instance.</param>
    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Builds and sends an HTML email to the specified recipient using SMTP with STARTTLS.
    /// Logs a warning on failure but does not rethrow, allowing the caller to continue.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The HTML body content of the email.</param>
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_config["SmtpSettings:SenderName"], _config["SmtpSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        try
        {
            using var smtp = new SmtpClient();
            var host = _config["SmtpSettings:Host"];
            var port = int.Parse(_config["SmtpSettings:Port"] ?? "587");
            
            _logger.LogInformation("Connecting to SMTP host {Host}:{Port}...", host, port);
            
            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            
            var username = _config["SmtpSettings:Username"];
            var password = _config["SmtpSettings:Password"];
            
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await smtp.AuthenticateAsync(username, password);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            // In a real app, you might want to retry or throw
        }
    }
}
