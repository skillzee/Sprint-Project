using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Interfaces;

/// <summary>
/// Defines the data access contract for user persistence operations.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Checks whether a user with the given email address already exists.
    /// </summary>
    /// <param name="email">The email address to look up.</param>
    /// <returns><c>true</c> if a user with that email exists; otherwise, <c>false</c>.</returns>
    Task<bool> UserExistsAsync(string email);

    /// <summary>
    /// Retrieves a user entity by their email address.
    /// </summary>
    /// <param name="email">The email address to query.</param>
    /// <returns>The matching <see cref="User"/>, or <c>null</c> if not found.</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Persists a new user entity to the database.
    /// </summary>
    /// <param name="user">The <see cref="User"/> entity to create.</param>
    /// <returns>The created <see cref="User"/> with its generated ID populated.</returns>
    Task<User> CreateUserAsync(User user);
}
