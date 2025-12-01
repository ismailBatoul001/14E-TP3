using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IBlockGeometryService
    {
        LineString ExtraireSegmentBlock(double lonDebut, double latDebut, double lonFin, double latFin, MultiLineString reseauRails);

    }
}
