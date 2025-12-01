using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Moq;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocomotivTests.Data.Repositories
{
    public class BlockGeometryServiceTest
    {
        [Fact]
        public void ExtraireSegmentBlock_RailTrouveNull_RetourneLigneVide()
        {
            var railServiceMock = new Mock<IRailGeometryService>();
            var service = new BlockGeometryService(railServiceMock.Object);

            var reseau = new MultiLineString(new LineString[0]);

            railServiceMock
                .Setup(r => r.FindClosestRailLine(It.IsAny<LineString[]>(), It.IsAny<Point>(), It.IsAny<Point>()))
                .Returns((LineString)null);

            var résultat = service.ExtraireSegmentBlock(0, 0, 1, 1, reseau);

            Assert.True(résultat.IsEmpty);
        }
    }
}
