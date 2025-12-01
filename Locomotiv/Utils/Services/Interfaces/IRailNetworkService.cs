using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;
using NetTopologySuite.Geometries;


namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IRailNetworkService
    {
        MultiLineString ChargerReseauFerroviaire(string geoJsonContent);
    }
}
