namespace TravelApp.Services.Notification.Services;

/// <summary>
/// Defines the contract for sending transactional emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an HTML email to the specified recipient.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The HTML body content of the email.</param>
    Task SendEmailAsync(string to, string subject, string body);
}
