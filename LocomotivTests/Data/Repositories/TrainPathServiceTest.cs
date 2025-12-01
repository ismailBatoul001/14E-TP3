using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Moq;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Mapsui;

namespace LocomotivTests.Data.Repositories
{
    public class TrainPathServiceTest
    {
        [Fact]
        public void CalculerCheminComplet_AjouteDesPointsAuChemin()
        {
            var railMock = new Mock<IRailGeometryService>();

            var service = new TrainPathService(railMock.Object);

            var rail = new LineString(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(10, 0)
            });

            var reseau = new MultiLineString(new[] { rail });
            service.SetReseau(reseau);

            var points = new List<(double lat, double lon, string nom)>
            {
                (0, 0, "A"),
                (0, 1, "B")
            };


            railMock.Setup(mockGeo => mockGeo.SnapPointToRail(It.IsAny<Point>(), It.IsAny<MultiLineString>())).Returns<Point, MultiLineString>((pointRecu, reseauRecu) => pointRecu); 
            
            var resultat = service.CalculerCheminComplet(points);

            Assert.True(resultat.Count > 0);
        }

        [Fact]
        public void TrouverCheminSurReseau_RailsConnectes_RetourneBonChemin()
        {
            var railMock = new Mock<IRailGeometryService>();

            var service = new TrainPathService(railMock.Object);

            var rail1 = new LineString(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(10, 0)
            });

            var rail2 = new LineString(new[]
            {
                new Coordinate(10, 0),
                new Coordinate(20, 0)
            });

            var reseau = new MultiLineString(new[] { rail1, rail2 });
            service.SetReseau(reseau);

            var depart = new Point(0, 0);
            var arrivee = new Point(20, 0);

            var chemin = service.TrouverCheminSurReseau(depart, arrivee);

            Assert.Equal(2, chemin.Count);
            Assert.Equal(rail1, chemin[0]);
            Assert.Equal(rail2, chemin[1]);
        }
    }
}
