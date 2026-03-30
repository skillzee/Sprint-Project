using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Interfaces;

public interface IAuthRepository
{
    Task<bool> UserExistsAsync(string email);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
}
