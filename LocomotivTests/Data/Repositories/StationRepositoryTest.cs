using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocomotivTests.Data.Repositories
{
    public class StationRepositoryTest : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IStationDAL _stationRepository;

        public StationRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _stationRepository = new StationDAL(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAll_ShouldReturnAllStations()
        {
            var station1 = new Station
            {
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            var station2 = new Station
            {
                Nom = "Gare Ouest",
                Latitude = 45.5088,
                Longitude = -73.554,
                CapaciteMaximale = 15
            };
            _context.Stations.AddRange(station1, station2);
            _context.SaveChanges();
            var stations = _stationRepository.GetAll().ToList();
            Assert.Equal(2, stations.Count);
        }

        [Fact]
        public void GetById_ShouldReturnStation()
        {
            var station = new Station
            {
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            _context.Stations.Add(station);
            _context.SaveChanges();

            var stationTrouve = _stationRepository.GetById(station.Id);
            Assert.NotNull(stationTrouve);
            Assert.Equal("Gare Centrale", stationTrouve.Nom);
            Assert.Equal(10, stationTrouve.CapaciteMaximale);
        }
    }
}
