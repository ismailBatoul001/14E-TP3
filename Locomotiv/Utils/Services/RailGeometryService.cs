using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Utils.Services.Interfaces;
using NetTopologySuite.Geometries;
using NetTopologySuite.LinearReferencing;

namespace Locomotiv.Utils.Services
{
    public class RailGeometryService : IRailGeometryService
    {

        public Point SnapPointToRail(Point point, MultiLineString railNetwork)
        {
            if (railNetwork == null || railNetwork.NumGeometries == 0) return point;

            LineString closestRail = null;
            double shortestDistance = double.MaxValue;

            foreach (LineString rail in railNetwork.Geometries)
            {
                double dist = rail.Distance(point);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestRail = rail;
                }
            }

            if (closestRail == null) return point;

            var lineIndex = new LengthIndexedLine(closestRail);
            double positionOnRail = lineIndex.Project(point.Coordinate);
            var snappedCoord = lineIndex.ExtractPoint(positionOnRail);
            return new Point(snappedCoord.X, snappedCoord.Y);
        }

        public LineString FindClosestRailLine(LineString[] railLines, Point start, Point end)
        {
            LineString bestLine = null;
            double bestScore = double.MaxValue;
            double directDistance = start.Distance(end);
            int validLines = 0;

            foreach (var rail in railLines)
            {
                if (rail.Coordinates.Length < 2) continue;

                double distStart = rail.Distance(start);
                double distEnd = rail.Distance(end);
                double threshold = Math.Max(5000, directDistance * 0.5);

                if (distStart > threshold || distEnd > threshold) continue;
                validLines++;

                double railLength = 0;
                for (int i = 0; i < rail.Coordinates.Length - 1; i++)
                {
                    var c1 = rail.Coordinates[i];
                    var c2 = rail.Coordinates[i + 1];
                    railLength += Math.Sqrt(Math.Pow(c2.X - c1.X, 2) + Math.Pow(c2.Y - c1.Y, 2));
                }

                double lengthPenalty = 0;
                if (railLength > directDistance * 1.5)
                    lengthPenalty = (railLength - directDistance) * 0.3;

                double score = distStart + distEnd + lengthPenalty;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestLine = rail;
                }
            }

            return bestLine;
        }
        public double GetLineLength(LineString line)
        {
            double total = 0.0;
            var coords = line.Coordinates;

            for (int i = 0; i < coords.Length - 1; i++)
            {
                double distanceX = coords[i + 1].X - coords[i].X;
                double distanceY = coords[i + 1].Y - coords[i].Y;
                total += Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            }

            return total;
        }



    }
}
