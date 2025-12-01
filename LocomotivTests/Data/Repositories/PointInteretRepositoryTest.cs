using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocomotivTests.Data.Repositories
{
    public class PointInteretRepositoryTest : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IPointInteretDAL _PIrepository;

        public PointInteretRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _PIrepository = new PointInteretDAL(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAll_ShouldReturnAllPointsInteret()
        {
            var pi1 = new PointInteret
            {
                Nom = "Monument A",
                Type = "Type A",
                Latitude = 45.5017,
                Longitude = -73.5673
            };
            var pi2 = new PointInteret
            {
                Nom = "Monument B",
                Type = "Type B",
                Latitude = 45.5088,
                Longitude = -73.554
            };
            _context.PointsInteret.AddRange(pi1, pi2);
            _context.SaveChanges();

            var pointsInteret = _PIrepository.GetAll().ToList();
            Assert.Equal(2, pointsInteret.Count);
        }

        [Fact]
        public void GetById_ShouldReturnPointInteret()
        {
            var PointInteret = new PointInteret
            {
                Nom = "Monument A",
                Type = "Type A",
                Latitude = 45.5017,
                Longitude = -73.5673
            };
            _context.PointsInteret.Add(PointInteret);
            _context.SaveChanges();

            var PointInteretRecu = _PIrepository.GetById(PointInteret.Id);

            Assert.NotNull(PointInteretRecu);
            Assert.Equal("Monument A", PointInteretRecu.Nom);
            Assert.Equal("Type A", PointInteretRecu.Type);
            Assert.Equal(45.5017, PointInteretRecu.Latitude);
            Assert.Equal(-73.5673, PointInteretRecu.Longitude);
        }
    }
}
