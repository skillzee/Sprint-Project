using Microsoft.EntityFrameworkCore;
using TravelApp.Services.Auth.Data;
using TravelApp.Services.Auth.Interfaces;
using TravelApp.Services.Auth.Models;

namespace TravelApp.Services.Auth.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _db;

    public AuthRepository(AuthDbContext db)
    {
        _db = db;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

}
