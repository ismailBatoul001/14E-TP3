using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IRailGeometryService
    {
        Point SnapPointToRail(Point point, MultiLineString railNetwork);

        LineString FindClosestRailLine(LineString[] railLines, Point start, Point end);

        double GetLineLength(LineString line);


    }
}
