using Locomotiv.Model;
using Mapsui;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface ITrainPathService
    {
        List<MPoint> CalculerCheminComplet(List<(double lat, double lon, string nom)> points);

        List<MPoint> CalculerSegmentSurRails(double lat1, double lon1, double lat2, double lon2);

        List<LineString> TrouverCheminSurReseau(Point depart, Point arrivee);

        void SetReseau(MultiLineString reseau);

    }
}
