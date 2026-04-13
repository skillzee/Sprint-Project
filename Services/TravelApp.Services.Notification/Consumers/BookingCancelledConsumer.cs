using MassTransit;
using TravelApp.Services.Notification.Services;
using TravelApp.Shared;

namespace TravelApp.Services.Notification.Consumers;

/// <summary>
/// MassTransit consumer that handles <see cref="BookingCancelledEvent"/> messages from RabbitMQ.
/// Sends a styled HTML booking cancellation email to the user.
/// </summary>
public class BookingCancelledConsumer : IConsumer<BookingCancelledEvent>
{
    private readonly ILogger<BookingCancelledConsumer> _logger;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of <see cref="BookingCancelledConsumer"/>.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="emailService">The email service used to send the cancellation email.</param>
    public BookingCancelledConsumer(ILogger<BookingCancelledConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// Processes a <see cref="BookingCancelledEvent"/> by sending a cancellation notification email
    /// with the booking reference and hotel name.
    /// </summary>
    /// <param name="context">The MassTransit consume context containing the event message.</param>
    public async Task Consume(ConsumeContext<BookingCancelledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing Booking Cancellation for {BookingRef}", message.BookingRef);

        var subject = $"Booking Cancelled: {message.BookingRef} - {message.HotelName}";
        
        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #e74c3c; color: white; padding: 20px; text-align: center;'>
                    <h1 style='margin: 0;'>Booking Cancelled</h1>
                </div>
                <div style='padding: 20px;'>
                    <p>Hi <strong>{message.UserName}</strong>,</p>
                    <p>We've processed your cancellation request for the following stay:</p>
                    
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='padding: 8px 0; color: #7f8c8d;'>Booking Reference:</td>
                            <td style='padding: 8px 0; text-align: right;'><strong>{message.BookingRef}</strong></td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #7f8c8d;'>Hotel:</td>
                            <td style='padding: 8px 0; text-align: right;'><strong>{message.HotelName}</strong></td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #7f8c8d;'>Status:</td>
                            <td style='padding: 8px 0; text-align: right; color: #e74c3c;'><strong>CANCELLED</strong></td>
                        </tr>
                    </table>
                    
                    <p style='margin-top: 20px; color: #34495e;'>If you didn't request this cancellation, please contact us immediately.</p>
                </div>
                <div style='background-color: #f1f1f1; padding: 15px; text-align: center; color: #95a5a6; font-size: 0.8em;'>
                    <p>&copy; {DateTime.UtcNow.Year} TravelApp Inc. All rights reserved.</p>
                </div>
            </div>";

        await _emailService.SendEmailAsync(message.UserEmail, subject, htmlBody);
    }
}
