using MassTransit;
using TravelApp.Services.Notification.Services;
using TravelApp.Shared;

namespace TravelApp.Services.Notification.Consumers;

/// <summary>
/// MassTransit consumer that handles <see cref="HotelRejectedEvent"/> messages from RabbitMQ.
/// Sends a styled HTML rejection notification email to the Hotel Manager, including the rejection reason if provided.
/// </summary>
public class HotelRejectedConsumer : IConsumer<HotelRejectedEvent>
{
    private readonly ILogger<HotelRejectedConsumer> _logger;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of <see cref="HotelRejectedConsumer"/>.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="emailService">The email service used to send the rejection email.</param>
    public HotelRejectedConsumer(ILogger<HotelRejectedConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// Processes a <see cref="HotelRejectedEvent"/> by sending a rejection notification email
    /// to the Hotel Manager. Includes the rejection reason section only when a reason is provided.
    /// </summary>
    /// <param name="context">The MassTransit consume context containing the event message.</param>
    public async Task Consume(ConsumeContext<HotelRejectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing Hotel Rejection notification for hotel {HotelId}", message.HotelId);

        var subject = $"Update on your hotel listing: '{message.HotelName}'";

        var reasonSection = string.IsNullOrWhiteSpace(message.Reason)
            ? ""
            : $@"<div style='padding: 15px; background-color: #fdf2f2; border-left: 4px solid #e74c3c; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Reason:</strong> {message.Reason}</p>
                 </div>";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #e74c3c; color: white; padding: 20px; text-align: center;'>
                    <h1 style='margin: 0;'>Hotel Listing Update</h1>
                </div>
                <div style='padding: 20px;'>
                    <p>Hi <strong>{message.OwnerName}</strong>,</p>
                    <p>After reviewing your hotel listing <strong>{message.HotelName}</strong>, our team was unable to approve it at this time.</p>
                    {reasonSection}
                    <p>You are welcome to update your listing and resubmit for review. If you have questions, please contact our support team.</p>
                </div>
                <div style='background-color: #f1f1f1; padding: 15px; text-align: center; color: #95a5a6; font-size: 0.8em;'>
                    <p>&copy; {DateTime.UtcNow.Year} VoyageIn. All rights reserved.</p>
                </div>
            </div>";

        await _emailService.SendEmailAsync(message.OwnerEmail, subject, htmlBody);
    }
}
