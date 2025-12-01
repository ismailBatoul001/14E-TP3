using Locomotiv.Utils.Services.Interfaces;
using Moq;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocomotivTests.Data.Repositories
{
    public class MockIRailGeometryServiceTest
    {
        [Fact]
        public void Mock_SnapPointToRail_RetourneLeMemePoint()
        {
            var geoMock = new Mock<IRailGeometryService>();

            geoMock.Setup(serviceGeo => serviceGeo.SnapPointToRail(It.IsAny<Point>(), It.IsAny<MultiLineString>())).Returns<Point, MultiLineString>((pointOriginal, reseauFerroviaire) => pointOriginal);

            var pointTest = new Point(3, 4);
            var reseauTest = new MultiLineString(new LineString[0]);

            var resultat = geoMock.Object.SnapPointToRail(pointTest, reseauTest);

            Assert.Equal(pointTest, resultat);
        }
    }
}
