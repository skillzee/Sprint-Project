namespace TravelApp.Services.Booking.DTOs;

/// <summary>
/// Lightweight DTO used by the Booking service to check a room's approval status
/// via an internal HTTP call to the Hotel service.
/// </summary>
/// <param name="Id">The room's unique identifier.</param>
/// <param name="ApprovalStatus">The room's current approval status (e.g., <c>"Approved"</c>, <c>"Pending"</c>, <c>"Rejected"</c>).</param>
public record RoomStatusDto(int Id, string ApprovalStatus);
