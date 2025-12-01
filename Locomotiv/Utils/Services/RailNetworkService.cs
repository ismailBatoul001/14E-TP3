using HarfBuzzSharp;
using Locomotiv.Utils.Services.Interfaces;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.LinearReferencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Locomotiv.Utils.Services
{
    public class RailNetworkService : IRailNetworkService
    {
        public MultiLineString ChargerReseauFerroviaire(string geoJsonContent)
        {
            if (string.IsNullOrWhiteSpace(geoJsonContent))
                throw new Exception("Le fichier GeoJSON est vide.");

            var lecteurGeoJson = new GeoJsonReader();
            var collection = lecteurGeoJson.Read<FeatureCollection>(geoJsonContent);

            var lignes = new List<LineString>();

            foreach (var feature in collection)
            {
                foreach (var ligne in ExtraireLignes(feature.Geometry))
                {
                    lignes.Add(ConvertirEnWebMercator(ligne));
                }
            }

            return new MultiLineString(lignes.ToArray());
        }

        private List<LineString> ExtraireLignes(Geometry geometrie)
        {
            var lignes = new List<LineString>();

            if (geometrie is LineString ligneUnique)
            {
                lignes.Add(ligneUnique);
            }
            else if (geometrie is MultiLineString multiLignes)
            {
                foreach (var multi in multiLignes.Geometries)
                {
                    if (multi is LineString sousLigne)
                    {
                        lignes.Add(sousLigne);
                    }
                }
            }

            return lignes;
        }

        private LineString ConvertirEnWebMercator(LineString ligne)
        {
            var convert = ligne.Coordinates
                .Select(coord =>
                {
                    var (x, y) = SphericalMercator.FromLonLat(coord.X, coord.Y);
                    return new Coordinate(x, y);
                })
                .ToArray();

            return new LineString(convert);
        }
    }
}
