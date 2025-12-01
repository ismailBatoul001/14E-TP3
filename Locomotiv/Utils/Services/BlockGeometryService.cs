using Locomotiv.Utils.Services.Interfaces;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.LinearReferencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services
{
    public class BlockGeometryService : IBlockGeometryService
    {
        private readonly IRailGeometryService _railService;

        public BlockGeometryService(IRailGeometryService railService)
        {
            _railService = railService;
        }

        public LineString ExtraireSegmentBlock(double lonDebut, double latDebut, double lonFin, double latFin, MultiLineString reseauRails)
        {
            var (mx1, my1) = SphericalMercator.FromLonLat(lonDebut, latDebut);
            var (mx2, my2) = SphericalMercator.FromLonLat(lonFin, latFin);

            var startPoint = new Point(mx1, my1);
            var endPoint = new Point(mx2, my2);

            var snapped1 = _railService.SnapPointToRail(startPoint, reseauRails);
            var snapped2 = _railService.SnapPointToRail(endPoint, reseauRails);

            var allLines = reseauRails.Geometries.Cast<LineString>().ToArray();
            var goodLine = _railService.FindClosestRailLine(allLines, snapped1, snapped2);
            if (goodLine == null)
                return new LineString(new Coordinate[0]); ;

            var locator = new LengthIndexedLine(goodLine);

            double loc1 = locator.Project(snapped1.Coordinate);
            double loc2 = locator.Project(snapped2.Coordinate);

            if (loc2 < loc1)
            {
                double t = loc1;
                loc1 = loc2;
                loc2 = t;
            }

            double lineLength = _railService.GetLineLength(goodLine);
            double extend = lineLength * 0.01 + 10;

            double start = Math.Max(0, loc1 - extend);
            double end = Math.Min(lineLength, loc2 + extend);

            var segment = locator.ExtractLine(start, end) as LineString;

            return segment ?? new LineString(Array.Empty<Coordinate>());
        }
    }
}
