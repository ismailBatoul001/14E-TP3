using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model.Interfaces;
using Moq;
using Locomotiv.Model;
using Locomotiv.Utils.Services;

namespace LocomotivTests.Data.Repositories
{
    public class PointInteretServiceTest
    {
        private readonly IPointInteretService _pointInteretService;
        private readonly Mock<IPointInteretDAL> _pointInteretRepositoryMock;
    
        public PointInteretServiceTest()
        {
            _pointInteretRepositoryMock = new Mock<IPointInteretDAL>();
            _pointInteretService = new PointInteretService(_pointInteretRepositoryMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllPointsInteret()
        {
            var PointsInteret = new List<PointInteret>
            {
                new PointInteret { Id = 1, Nom = "Monument A", Type = "Type A", Latitude = 45.5017, Longitude = -73.5673 },
                new PointInteret { Id = 2, Nom = "Monument B", Type = "Type B", Latitude = 45.5088, Longitude = -73.554 }
            };

            _pointInteretRepositoryMock.Setup(repo => repo.GetAll()).Returns(PointsInteret);
            var ComptePointsInterets = _pointInteretService.GetAll();

            Assert.Equal(PointsInteret.Count, ComptePointsInterets.Count());
            for (int i = 0; i < PointsInteret.Count; i++)
            {
                Assert.Equal(PointsInteret[i].Id, ComptePointsInterets.ElementAt(i).Id);
                Assert.Equal(PointsInteret[i].Nom, ComptePointsInterets.ElementAt(i).Nom);
                Assert.Equal(PointsInteret[i].Type, ComptePointsInterets.ElementAt(i).Type);
                Assert.Equal(PointsInteret[i].Latitude, ComptePointsInterets.ElementAt(i).Latitude);
                Assert.Equal(PointsInteret[i].Longitude, ComptePointsInterets.ElementAt(i).Longitude);
            }
        }

        [Fact]
        public void GetById_ShouldReturnPointInteret()
        {
            var PointInteret = new PointInteret
            {
                Id = 1,
                Nom = "Monument A",
                Type = "Type A",
                Latitude = 45.5017,
                Longitude = -73.5673
            };

            _pointInteretRepositoryMock.Setup(repo => repo.GetById(PointInteret.Id)).Returns(PointInteret);
            var PointInteretRecu = _pointInteretService.GetById(PointInteret.Id);

            Assert.NotNull(PointInteretRecu);
            Assert.Equal(PointInteret.Id, PointInteretRecu.Id);
            Assert.Equal(PointInteret.Nom, PointInteretRecu.Nom);
            Assert.Equal(PointInteret.Type, PointInteretRecu.Type);
            Assert.Equal(PointInteret.Latitude, PointInteretRecu.Latitude);
            Assert.Equal(PointInteret.Longitude, PointInteretRecu.Longitude);
        }

        [Fact]
        public void GetById_ThrowException_WhenPointInteretNotFound()
        {
            int invalidId = 999;

            _pointInteretRepositoryMock.Setup(repo => repo.GetById(invalidId)).Returns((PointInteret)null);

            Assert.Throws<Exception>(() => _pointInteretService.GetById(invalidId));
        }
    }
}
