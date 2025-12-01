using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocomotivTests.Data.Repositories
{
    public class UserRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly UserDAL _userDAL;

        public UserRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _userDAL = new UserDAL(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void FindByUsernameAndPassword_ShouldReturnUser()
        {
            var station = new Station
            {
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            var user = new User
            {
                Prenom = "test",
                Nom = "user",
                Username = "testuser",
                Password = "password123",
                Role = Role.Employe,
                StationAssigneeId = station.Id
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var foundUser = _userDAL.FindByUsernameAndPassword("testuser", "password123");
            Assert.NotNull(foundUser);
            Assert.Equal("testuser", foundUser.Username);
            Assert.Equal(Role.Employe, foundUser.Role);
        }
    }
}
