using System.Collections.Generic;
using System.Linq;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Locomotiv.Model.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly ApplicationDbContext _context;

        public UserDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        public User? FindByUsernameAndPassword(string u, string p)
        {
            return _context.Users.FirstOrDefault(u2 => u2.Username == u && u2.Password == p);
        }
    }
}
