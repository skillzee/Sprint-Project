using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Auth.Data;
using TravelApp.Services.Auth.Interfaces;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Repositories;

/// <summary>
/// Implements data access for user entities using Entity Framework Core against the Auth SQL database.
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _db;

    /// <summary>
    /// Initializes a new instance of <see cref="AuthRepository"/>.
    /// </summary>
    /// <param name="db">The EF Core database context for the Auth service.</param>
    public AuthRepository(AuthDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Persists a new user to the database and saves changes.
    /// </summary>
    /// <param name="user">The user entity to save.</param>
    /// <returns>The saved user with its generated database ID.</returns>
    public async Task<User> CreateUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Finds and returns a user by their email address.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <returns>The matching <see cref="User"/>, or <c>null</c> if not found.</returns>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Checks if any user record exists with the specified email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns><c>true</c> if a user exists with that email; otherwise, <c>false</c>.</returns>
    public async Task<bool> UserExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }
}
