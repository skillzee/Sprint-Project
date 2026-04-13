using MassTransit;
using TravelApp.Services.Notification.Services;
using TravelApp.Shared;

namespace TravelApp.Services.Notification.Consumers;

/// <summary>
/// MassTransit consumer that handles <see cref="BookingConfirmedEvent"/> messages from RabbitMQ.
/// Sends a styled HTML booking confirmation email to the user.
/// </summary>
public class BookingConfirmedConsumer : IConsumer<BookingConfirmedEvent>
{
    private readonly ILogger<BookingConfirmedConsumer> _logger;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of <see cref="BookingConfirmedConsumer"/>.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="emailService">The email service used to send the confirmation email.</param>
    public BookingConfirmedConsumer(ILogger<BookingConfirmedConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// Processes a <see cref="BookingConfirmedEvent"/> by sending a booking confirmation email
    /// with the booking reference, hotel name, room type, dates, and total price.
    /// </summary>
    /// <param name="context">The MassTransit consume context containing the event message.</param>
    public async Task Consume(ConsumeContext<BookingConfirmedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing Booking Confirmation for {BookingRef}", message.BookingRef);

        var subject = $"Booking Confirmed: {message.BookingRef} - {message.HotelName}";
        
        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #2c3e50; color: white; padding: 20px; text-align: center;'>
                    <h1 style='margin: 0;'>Booking Confirmed!</h1>
                </div>
                <div style='padding: 20px;'>
                    <p>Hi <strong>{message.UserName}</strong>,</p>
                    <p>Your booking has been successfully confirmed. Here are your trip details:</p>
                    
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
                            <td style='padding: 8px 0; color: #7f8c8d;'>Room Type:</td>
                            <td style='padding: 8px 0; text-align: right;'>{message.RoomType}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #7f8c8d;'>Check-in:</td>
                            <td style='padding: 8px 0; text-align: right;'>{message.CheckInDate:MMM dd, yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #7f8c8d;'>Check-out:</td>
                            <td style='padding: 8px 0; text-align: right;'>{message.CheckOutDate:MMM dd, yyyy}</td>
                        </tr>
                        <tr style='border-top: 2px solid #e0e0e0;'>
                            <td style='padding: 15px 0; font-size: 1.2em;'>Total Paid:</td>
                            <td style='padding: 15px 0; text-align: right; font-size: 1.2em; color: #27ae60;'><strong>{message.TotalPrice:C}</strong></td>
                        </tr>
                    </table>
                    
                    <div style='margin-top: 30px; padding: 20px; background-color: #f9f9f9; border-left: 4px solid #3498db;'>
                        <p style='margin: 0; font-size: 0.9em; color: #34495e;'>
                            <strong>Need help?</strong> Contact our support team if you have any questions regarding your stay.
                        </p>
                    </div>
                </div>
                <div style='background-color: #f1f1f1; padding: 15px; text-align: center; color: #95a5a6; font-size: 0.8em;'>
                    <p>&copy; {DateTime.UtcNow.Year} TravelApp Inc. All rights reserved.</p>
                </div>
            </div>";

        await _emailService.SendEmailAsync(message.UserEmail, subject, htmlBody);
    }
}
