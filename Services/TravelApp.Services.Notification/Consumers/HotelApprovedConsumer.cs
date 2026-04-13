using MassTransit;
using TravelApp.Services.Notification.Services;
using TravelApp.Shared;

namespace TravelApp.Services.Notification.Consumers;

/// <summary>
/// MassTransit consumer that handles <see cref="HotelApprovedEvent"/> messages from RabbitMQ.
/// Sends a styled HTML approval notification email to the Hotel Manager.
/// </summary>
public class HotelApprovedConsumer : IConsumer<HotelApprovedEvent>
{
    private readonly ILogger<HotelApprovedConsumer> _logger;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of <see cref="HotelApprovedConsumer"/>.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="emailService">The email service used to send the approval email.</param>
    public HotelApprovedConsumer(ILogger<HotelApprovedConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// Processes a <see cref="HotelApprovedEvent"/> by sending an approval notification email
    /// to the Hotel Manager informing them their listing is now live.
    /// </summary>
    /// <param name="context">The MassTransit consume context containing the event message.</param>
    public async Task Consume(ConsumeContext<HotelApprovedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing Hotel Approval notification for hotel {HotelId}", message.HotelId);

        var subject = $"Your hotel '{message.HotelName}' has been approved!";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #27ae60; color: white; padding: 20px; text-align: center;'>
                    <h1 style='margin: 0;'>Hotel Approved!</h1>
                </div>
                <div style='padding: 20px;'>
                    <p>Hi <strong>{message.OwnerName}</strong>,</p>
                    <p>Great news! Your hotel listing has been reviewed and approved by our team.</p>
                    <div style='padding: 15px; background-color: #eafaf1; border-left: 4px solid #27ae60; margin: 20px 0;'>
                        <p style='margin: 0; font-size: 1.1em;'><strong>{message.HotelName}</strong> is now live and visible to customers.</p>
                    </div>
                    <p>Customers can now discover and book rooms at your property through VoyageIn.</p>
                </div>
                <div style='background-color: #f1f1f1; padding: 15px; text-align: center; color: #95a5a6; font-size: 0.8em;'>
                    <p>&copy; {DateTime.UtcNow.Year} VoyageIn. All rights reserved.</p>
                </div>
            </div>";

        await _emailService.SendEmailAsync(message.OwnerEmail, subject, htmlBody);
    }
}
