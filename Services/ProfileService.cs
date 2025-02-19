using be_magang.Models;
using be_magang.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace be_magang.Services
{
    public interface IProfileService
    {
        Task<User> GetProfile(int userId);
        Task<User> UpdateProfile(int userId, string name, string email);
    }

    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetProfile(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> UpdateProfile(int userId, string name, string email)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            user.Name = name;
            user.Email = email;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
