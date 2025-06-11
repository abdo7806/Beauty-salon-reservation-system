using BookingSystem.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BookingDbContext _context;

        public AuthRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)// ارجاع المستخدم حسب الايمال
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)// انشاء مستخدم
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ChangeUserPassword(User user)
        {

            try
            {
                _context.Users.Update(user);

                await _context.SaveChangesAsync();
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }



        }


    }

}
