using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LocomotivTests.Services
{
    public class PlanificationItineraireServiceTest
    {
        private readonly Mock<ITrainRepository> _trainRepoMock;
        private readonly Mock<IStationDAL> _stationRepoMock;
        private readonly Mock<IItineraireService> _itineraireServiceMock;
        private readonly Mock<IPointInteretDAL> _pointInteretMock;

        private readonly IPlanificationItineraireService _service;

        public PlanificationItineraireServiceTest()
        {
            _trainRepoMock = new Mock<ITrainRepository>();
            _stationRepoMock = new Mock<IStationDAL>();
            _itineraireServiceMock = new Mock<IItineraireService>();
            _pointInteretMock = new Mock<IPointInteretDAL>();

            _service = new PlanificationItineraireService(
                _trainRepoMock.Object,
                _stationRepoMock.Object,
                _itineraireServiceMock.Object,
                _pointInteretMock.Object
            );
        }


        [Fact]
        public void ObtenirTrainsDisponibles_ShouldReturnOnlyAvailableTrains()
        {
            var trains = new List<Train>
            {
                new Train { Id = 1, Etat = EtatTrain.EnGare },
                new Train { Id = 2, Etat = EtatTrain.HorsService },
                new Train { Id = 3, Etat = EtatTrain.EnTransit },
                new Train { Id = 4, Etat = EtatTrain.EnGare }
            };

            _trainRepoMock.Setup(r => r.GetAll()).Returns(trains);

            var disponibles = _service.ObtenirTrainsDisponibles().ToList();

            Assert.Equal(2, disponibles.Count);
            Assert.Contains(disponibles, t => t.Id == 1);
            Assert.Contains(disponibles, t => t.Id == 4);
        }

        [Fact]
        public void ObtenirToutesLesStations_ShouldReturnList()
        {
            var stations = new List<Station>
            {
                new Station { Id = 10 },
                new Station { Id = 11 }
            };

            _stationRepoMock.Setup(s => s.GetAll()).Returns(stations);

            var result = _service.ObtenirToutesLesStations().ToList();

            Assert.Equal(2, result.Count);
        }


        [Fact]
        public void ObtenirTousLesPointsInteret_ShouldReturnList()
        {
            var points = new List<PointInteret>
            {
                new PointInteret { Id = 1 },
                new PointInteret { Id = 2 }
            };

            _pointInteretMock.Setup(p => p.GetAll()).Returns(points);

            var result = _service.ObtenirTousLesPointsInteret().ToList();

            Assert.Equal(2, result.Count);
        }


        [Fact]
        public void ValiderItineraire_ShouldFail_WhenTrainIsNull()
        {
            var ok = _service.ValiderItineraire(null, new Station(), new Station(), out string erreur);

            Assert.False(ok);
            Assert.Equal("Veuillez sélectionner un train.", erreur);
        }

        [Fact]
        public void ValiderItineraire_ShouldFail_WhenStationDepartIsNull()
        {
            var ok = _service.ValiderItineraire(new Train(), null, new Station(), out string erreur);

            Assert.False(ok);
            Assert.Equal("Veuillez sélectionner une station de départ.", erreur);
        }

        [Fact]
        public void ValiderItineraire_ShouldFail_WhenStationArriveeIsNull()
        {
            var ok = _service.ValiderItineraire(new Train(), new Station(), null, out string erreur);

            Assert.False(ok);
            Assert.Equal("Veuillez sélectionner une station d'arrivée.", erreur);
        }

        [Fact]
        public void ValiderItineraire_ShouldFail_WhenStationsAreSame()
        {
            var station = new Station { Id = 10 };

            var ok = _service.ValiderItineraire(new Train(), station, station, out string erreur);

            Assert.False(ok);
            Assert.Equal("Les stations de départ et d'arrivée doivent être différentes.", erreur);
        }

        [Fact]
        public void ValiderItineraire_ShouldFail_WhenTrainIsHorsService()
        {
            var train = new Train { Etat = EtatTrain.HorsService };

            var ok = _service.ValiderItineraire(train, new Station(), new Station { Id = 2 }, out string erreur);

            Assert.False(ok);
            Assert.Equal("Le train est hors service.", erreur);
        }

        [Fact]
        public void ValiderItineraire_ShouldFail_WhenTrainIsEnTransit()
        {
            var train = new Train { Etat = EtatTrain.EnTransit };

            var ok = _service.ValiderItineraire(train, new Station(), new Station { Id = 2 }, out string erreur);

            Assert.False(ok);
            Assert.Equal("Le train est déjà en transit.", erreur);
        }

        [Fact]
        public void ValiderItineraire_ShouldPass_WhenValid()
        {
            var train = new Train { Etat = EtatTrain.EnGare };

            var ok = _service.ValiderItineraire(train, new Station { Id = 1 }, new Station { Id = 2 }, out string erreur);

            Assert.True(ok);
            Assert.Equal("", erreur);
        }


        [Fact]
        public void ConstruireListeArrets_ShouldReturnEmpty_WhenNull()
        {
            var result = _service.ConstruireListeArrets(null);

            Assert.Empty(result);
        }

        [Fact]
        public void ConstruireListeArrets_ShouldAddStation()
        {
            var station = new Station { Id = 10 };

            var result = _service.ConstruireListeArrets(new List<object> { station });

            Assert.Single(result);
            Assert.True(result[0].EstStation);
            Assert.Equal(10, result[0].StationId);
        }

        [Fact]
        public void ConstruireListeArrets_ShouldAddPointInteret()
        {
            var point = new PointInteret { Id = 20 };

            var result = _service.ConstruireListeArrets(new List<object> { point });

            Assert.Single(result);
            Assert.False(result[0].EstStation);
            Assert.Equal(20, result[0].PointInteretId);
        }

        [Fact]
        public void ConstruireListeArrets_ShouldHandleMixedElements()
        {
            var elements = new List<object>
            {
                new Station { Id = 10 },
                new PointInteret { Id = 20 }
            };

            var result = _service.ConstruireListeArrets(elements);

            Assert.Equal(2, result.Count);

            Assert.True(result[0].EstStation);
            Assert.False(result[1].EstStation);
        }

        [Fact]
        public void CreerNouvelItineraire_ShouldThrow_WhenTrainIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _service.CreerNouvelItineraire(null, 1, 2, new List<ItineraireArret>())
            );
        }

        [Fact]
        public void CreerNouvelItineraire_ShouldThrow_WhenDepartIdInvalid()
        {
            var train = new Train();

            Assert.Throws<ArgumentException>(() =>
                _service.CreerNouvelItineraire(train, 0, 2, new List<ItineraireArret>())
            );
        }

        [Fact]
        public void CreerNouvelItineraire_ShouldThrow_WhenArriveeIdInvalid()
        {
            var train = new Train();

            Assert.Throws<ArgumentException>(() =>
                _service.CreerNouvelItineraire(train, 1, -5, new List<ItineraireArret>())
            );
        }

        [Fact]
        public void CreerNouvelItineraire_ShouldCallInnerService()
        {
            var train = new Train { Id = 1 };
            var arrets = new List<ItineraireArret>();

            _itineraireServiceMock
                .Setup(s => s.CreerItineraire(train, 1, 2, arrets))
                .Returns(new Itineraire { Id = 99 });

            var itin = _service.CreerNouvelItineraire(train, 1, 2, arrets);

            Assert.Equal(99, itin.Id);
            _itineraireServiceMock.Verify(s =>
                s.CreerItineraire(train, 1, 2, arrets), Times.Once);
        }
        [Fact]
        public void DemarrerItineraire_ShouldThrow_WhenInvalidId()
        {
            Assert.Throws<ArgumentException>(() => _service.DemarrerItineraire(0));
        }

        [Fact]
        public void DemarrerItineraire_ShouldCallInnerService()
        {
            _service.DemarrerItineraire(10);

            _itineraireServiceMock.Verify(s =>
                s.DemarrerItineraire(10), Times.Once);
        }
    }
}
