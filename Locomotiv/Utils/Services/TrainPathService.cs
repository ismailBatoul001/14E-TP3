using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;
using Mapsui;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using NetTopologySuite.LinearReferencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services
{
    public class TrainPathService : ITrainPathService
    {
        private readonly IRailGeometryService _railService;
        private MultiLineString _railReseau;

        public TrainPathService(IRailGeometryService railService)
        {
            _railService = railService;
        }

        public void SetReseau(MultiLineString reseau)
        {
            _railReseau = reseau;
        }

        public List<MPoint> CalculerCheminComplet(List<(double lat, double lon, string nom)> points)
        {
            var cheminComplet = new List<MPoint>();
            if (points.Count < 2 || _railReseau == null)
                return cheminComplet;

            for (int i = 0; i < points.Count - 1; i++)
            {
                var a = points[i];
                var b = points[i + 1];


                var segment = CalculerSegmentSurRails(a.lat, a.lon, b.lat, b.lon);

                if (cheminComplet.Count > 0 && segment.Count > 0)
                {
                    var dist = Distance(cheminComplet.Last(), segment.First());
                    if (dist < 10) segment.RemoveAt(0);
                }

                cheminComplet.AddRange(segment);
            }

            return cheminComplet;
        }

        public List<MPoint> CalculerSegmentSurRails(double lat1, double lon1, double lat2, double lon2)
        {
            var resultat = new List<MPoint>();
            if (_railReseau == null) return resultat;

            var (mx1, my1) = SphericalMercator.FromLonLat(lon1, lat1);
            var (mx2, my2) = SphericalMercator.FromLonLat(lon2, lat2);

            var depart = new Point(mx1, my1);
            var arrivee = new Point(mx2, my2);

            var rails = TrouverCheminSurReseau(depart, arrivee);
            if (rails.Count == 0) return resultat;

            for (int i = 0; i < rails.Count; i++)
            {
                var rail = rails[i];
                var locator = new LengthIndexedLine(rail);

                double loc1, loc2;

                if (i == 0)
                {
                    loc1 = locator.Project(depart.Coordinate);
                    loc2 = locator.EndIndex;
                }
                else if (i == rails.Count - 1)
                {
                    loc1 = locator.StartIndex;
                    loc2 = locator.Project(arrivee.Coordinate);
                }
                else
                {
                    loc1 = locator.StartIndex;
                    loc2 = locator.EndIndex;
                }

                if (loc2 < loc1) (loc1, loc2) = (loc2, loc1);

                var part = locator.ExtractLine(loc1, loc2);
                foreach (var c in part.Coordinates)
                    resultat.Add(new MPoint(c.X, c.Y));
            }

            return resultat;
        }

        public List<LineString> TrouverCheminSurReseau(Point depart, Point arrivee)
        {
            if (_railReseau == null)
                return new List<LineString>();

            var allLines = _railReseau.Geometries.Cast<LineString>().ToList();

            LineString railDepart = null;
            LineString railArrivee = null;

            double minD1 = double.MaxValue;
            double minD2 = double.MaxValue;

            foreach (var r in allLines)
            {
                double d1 = r.Distance(depart);
                double d2 = r.Distance(arrivee);

                if (d1 < minD1) { minD1 = d1; railDepart = r; }
                if (d2 < minD2) { minD2 = d2; railArrivee = r; }
            }

            if (railDepart == null || railArrivee == null)
                return new List<LineString>();

            if (railDepart == railArrivee)
                return new List<LineString> { railDepart };

            var visited = new HashSet<LineString>();
            var queue = new Queue<(LineString, List<LineString>)>();

            visited.Add(railDepart);
            queue.Enqueue((railDepart, new List<LineString> { railDepart }));

            while (queue.Count > 0)
            {
                var (current, path) = queue.Dequeue();

                foreach (var voisin in allLines)
                {
                    if (visited.Contains(voisin)) continue;

                    if (RailsConnectes(current, voisin))
                    {
                        var nouveauChemin = new List<LineString>(path) { voisin };

                        if (voisin == railArrivee)
                            return nouveauChemin;

                        visited.Add(voisin);
                        queue.Enqueue((voisin, nouveauChemin));
                    }
                }
            }

            return new List<LineString> { railDepart };
        }

        private bool RailsConnectes(LineString a, LineString b)
        {
            const double seuil = 50;

            var A1 = a.StartPoint.Coordinate;
            var A2 = a.EndPoint.Coordinate;

            var B1 = b.StartPoint.Coordinate;
            var B2 = b.EndPoint.Coordinate;

            return Distance(A1, B1) < seuil ||
                   Distance(A1, B2) < seuil ||
                   Distance(A2, B1) < seuil ||
                   Distance(A2, B2) < seuil;
        }

        private double Distance(Coordinate c1, Coordinate c2)
            => Math.Sqrt(Math.Pow(c1.X - c2.X, 2) + Math.Pow(c1.Y - c2.Y, 2));

        private double Distance(MPoint p1, MPoint p2)
            => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
}
