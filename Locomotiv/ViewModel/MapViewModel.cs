using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Mapsui;
using Mapsui.Extensions.Provider;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.LinearReferencing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Locomotiv.ViewModel
{

    public class MapViewModel : BaseViewModel
    {
        private readonly IStationDAL _stationDAL;
        private readonly IPointInteretDAL _pointInteretDAL;
        private readonly BlockDAL _blockDAL;

        private readonly IRailNetworkService _railNetworkService;
        private readonly IRailGeometryService _railGeometryService;
        private readonly IBlockGeometryService _blockGeometryService;
        private readonly ITrainPathService _trainPathService;

        private MultiLineString _railReseau;
        private DispatcherTimer _trainTimer;
        private MemoryLayer _trainsLayer;
        private List<IFeature> _trainFeatures;

        private readonly Dictionary<int, List<MPoint>> _trainPaths = new();
        private readonly Dictionary<int, int> _trainPositionIndices = new();

        public IStationDAL StationDAL => _stationDAL;
        public ObservableCollection<PointInteret> Points { get; set; }
        public ObservableCollection<Station> Stations { get; set; }
        public Map Map { get; set; }


        public MapViewModel(IStationDAL stationDAL,
        IPointInteretDAL pointInteretDAL,
        BlockDAL blockDAL,
        IRailNetworkService railNetworkService,
        IRailGeometryService railGeometryService,
        IBlockGeometryService blockGeometryService,
        ITrainPathService trainPathService)
        {
            _stationDAL = stationDAL;
            _pointInteretDAL = pointInteretDAL;
            _blockDAL = blockDAL;




            _railNetworkService = railNetworkService;
            _railGeometryService = railGeometryService;
            _blockGeometryService = blockGeometryService;
            _trainPathService = trainPathService;

            Points = new ObservableCollection<PointInteret>(_pointInteretDAL.GetAll());
            Stations = new ObservableCollection<Station>(_stationDAL.GetAll());

            InitialiserCarte();
        }

        private void InitialiserCarte()
        {
            Map = CreateBaseMap();

            string geoJson = ResourceHelper.LireRessourceTexte("Locomotiv.Assets.Json.ReseauFerroviaire_clean.geojson");


            _railReseau = _railNetworkService.ChargerReseauFerroviaire(geoJson);

            _trainPathService.SetReseau(_railReseau);


            AjouterLayerRail();
            AjouterLayerPoints();
            AjouterLayerStations();
            AjouterLayerBlocks();


            InitialiserLayerTrains();
            ZoomMap();
            DemarrerTimer();
        }

        private Map CreateBaseMap()
        {
            var map = new Map();
            map.Widgets.Clear();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            return map;
        }

        private (ImageStyle, SymbolStyle, ImageStyle, SymbolStyle) CreateSymbolStyles(Color bleu, Color rouge)
        {
            var pointStyle = new SymbolStyle
            {
                SymbolType = SymbolType.Ellipse,
                Fill = null,
                Outline = new Pen(bleu, 3),
                SymbolScale = 1.2
            };

            var stationStyle = new SymbolStyle
            {
                SymbolType = SymbolType.Ellipse,
                Fill = null,
                Outline = new Pen(rouge, 3),
                SymbolScale = 1.2
            };

            var pointImageStyle = new ImageStyle
            {
                Image = new Mapsui.Styles.Image
                {
                    Source = "embedded://Locomotiv.Assets.Icons.pointArret.png"
                },
                SymbolScale = 0.07
            };

            var stationImageStyle = new ImageStyle
            {
                Image = new Mapsui.Styles.Image
                {
                    Source = "embedded://Locomotiv.Assets.Icons.station.png"
                },
                SymbolScale = 0.03
            };

            return (pointImageStyle, pointStyle, stationImageStyle, stationStyle);
        }

        private void AjouterLayerRail()
        {
            var railFeatures = _railReseau.Geometries
                .Cast<LineString>()
                .Select(ls => new GeometryFeature { Geometry = ls })
                .ToList<IFeature>();

            var railLayer = new MemoryLayer
            {
                Name = "Réseau ferroviaire",
                Features = railFeatures,
                Style = new VectorStyle
                {
                    Line = new Pen(Color.FromString("#000000"), 2f),
                    Fill = null,
                    Outline = null
                },
                Enabled = true,
                Opacity = 1.0
            };

            Map.Layers.Add(railLayer);
        }

        private void AjouterLayerPoints()
        {
            var (img, style, _, _) = CreateSymbolStyles(
               Color.FromArgb(255, 0, 0, 255),
               Color.FromArgb(255, 255, 0, 0));

            var pointFeatures = Points.Select(point =>
            {
                var (mx, my) = SphericalMercator.FromLonLat(point.Longitude, point.Latitude);
                var snapped = _railGeometryService.SnapPointToRail(new Point(mx, my), _railReseau);

                return new PointFeature(new MPoint(snapped.X, snapped.Y))
                {
                    ["Name"] = point.Nom,
                    ["Type"] = point.Type,
                    Styles = new List<IStyle> { img, style }
                };
            }).ToList();

            Map.Layers.Add(new MemoryLayer
            {
                Name = "Points d'intérêt",
                Features = pointFeatures,
                Opacity = 2.0
            });
        }

        private void AjouterLayerStations()
        {
            var (_, _, img, style) = CreateSymbolStyles(
                Color.FromArgb(255, 0, 0, 255),
                Color.FromArgb(255, 255, 0, 0));

            var stationFeatures = Stations.Select(station =>
            {
                var (mx, my) = SphericalMercator.FromLonLat(station.Longitude, station.Latitude);
                var snapped = _railGeometryService.SnapPointToRail(new Point(mx, my), _railReseau);

                return new PointFeature(new MPoint(snapped.X, snapped.Y))
                {
                    ["Id"] = station.Id.ToString(),
                    ["Name"] = station.Nom,
                    ["Type"] = "Gare",
                    Styles = new List<IStyle> { img, style }
                };
            }).ToList();

            Map.Layers.Add(new MemoryLayer
            {
                Name = "Gares",
                Features = stationFeatures,

            });
        }

        private void AjouterLayerBlocks()
        {
            var blockFeatures = new List<IFeature>();

            foreach (var block in _blockDAL.GetAll())
            {
                var segment = _blockGeometryService.ExtraireSegmentBlock(
                    block.LongitudeDebut,
                    block.LatitudeDebut,
                    block.LongitudeFin,
                    block.LatitudeFin,
                    _railReseau
                );

                if (segment == null || segment.IsEmpty)
                    continue;

                var feature = new GeometryFeature { Geometry = segment };
                feature["Nom"] = block.Nom;
                feature["Occupe"] = block.EstOccupe;
                blockFeatures.Add(feature);
            }

            Map.Layers.Add(new MemoryLayer
            {
                Name = "Blocks",
                Features = blockFeatures,
                Style = new VectorStyle
                {
                    Line = new Pen(Color.FromString("#1E90FF"), 3.0f),
                    Fill = null,
                    Outline = null
                },
                Enabled = true,
                Opacity = 1.0
            });

        }
        private void InitialiserLayerTrains()
        {
            _trainFeatures = new List<IFeature>();

            _trainsLayer = new MemoryLayer
            {
                Name = "Trains en mouvement",
                Features = _trainFeatures,
                Enabled = true,
                Opacity = 1.0,
                Style = new SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Brush(Color.FromArgb(255, 255, 0, 0)),
                    Outline = new Pen(Color.FromArgb(255, 0, 0, 0), 3),
                    SymbolScale = 0.5
                }
            };

            Map.Layers.Add(_trainsLayer);
        }


        private void DemarrerTimer()
        {
            _trainTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _trainTimer.Tick += MettreAJourPositionsTrains;
            _trainTimer.Start();
        }

        public void ArreterTimer()
        {
            _trainTimer?.Stop();
        }

        private void MettreAJourPositionsTrains(object sender, EventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
                var trainsEnTransit = context.Trains
                    .Include(t => t.StationActuelle)
                    .Include(t => t.BlockActuel)
                    .Where(t => t.Etat == EtatTrain.EnTransit)
                    .ToList();

                var newTrainFeatures = new List<IFeature>();

                foreach (var train in trainsEnTransit)
                {
                    var itineraire = context.Itineraires
                        .Include(i => i.StationDepart)
                        .Include(i => i.StationArrivee)
                        .Include(i => i.Arrets.OrderBy(a => a.Ordre))
                            .ThenInclude(a => a.Station)
                        .Include(i => i.Arrets)
                            .ThenInclude(a => a.PointInteret)
                        .FirstOrDefault(i => i.TrainId == train.Id && i.EstActif);

                    if (itineraire == null)
                        continue;

                    if (!_trainPaths.ContainsKey(train.Id))
                    {

                        var pointsTrajet = new List<(double lat, double lon, string nom)>();

                        pointsTrajet.Add((
                            itineraire.StationDepart.Latitude,
                            itineraire.StationDepart.Longitude,
                            itineraire.StationDepart.Nom
                        ));

                        foreach (var arret in itineraire.Arrets.OrderBy(a => a.Ordre))
                        {
                            if (arret.EstStation && arret.Station != null)
                            {
                                pointsTrajet.Add((
                                    arret.Station.Latitude,
                                    arret.Station.Longitude,
                                    arret.Station.Nom
                                ));
                            }
                            else if (!arret.EstStation && arret.PointInteret != null)
                            {
                                pointsTrajet.Add((
                                    arret.PointInteret.Latitude,
                                    arret.PointInteret.Longitude,
                                    arret.PointInteret.Nom
                                ));
                            }
                        }

                        pointsTrajet.Add((
                            itineraire.StationArrivee.Latitude,
                            itineraire.StationArrivee.Longitude,
                            itineraire.StationArrivee.Nom
                        ));

                        var chemin = _trainPathService.CalculerCheminComplet(pointsTrajet);

                        if (chemin.Count == 0)
                        {
                            continue;
                        }

                        _trainPaths[train.Id] = chemin;
                        _trainPositionIndices[train.Id] = 0;
                    }

                    var trainPath = _trainPaths[train.Id];
                    int indexActuel = _trainPositionIndices[train.Id];

                    if (indexActuel >= trainPath.Count)
                    {

                        if (train.BlockActuelId.HasValue)
                        {
                            var blockActuel = context.Blocks.Find(train.BlockActuelId.Value);
                            if (blockActuel != null)
                            {
                                blockActuel.EstOccupe = false;
                                blockActuel.TrainActuelId = null;
                            }
                        }

                        train.Etat = EtatTrain.EnGare;
                        train.StationActuelleId = itineraire.StationArriveeId;
                        train.BlockActuelId = null;
                        itineraire.EstActif = false;

                        context.Trains.Update(train);
                        context.Itineraires.Update(itineraire);
                        context.SaveChanges();

                        _trainPaths.Remove(train.Id);
                        _trainPositionIndices.Remove(train.Id);
                        continue;
                    }

                    var position = trainPath[indexActuel];

                    var blockActuelId = TrouverBlockParPosition(position, context);

                    if (blockActuelId.HasValue && blockActuelId != train.BlockActuelId)
                    {
                        if (train.BlockActuelId.HasValue)
                        {
                            var ancienBlock = context.Blocks.Find(train.BlockActuelId.Value);
                            if (ancienBlock != null)
                            {
                                ancienBlock.EstOccupe = false;
                                ancienBlock.TrainActuelId = null;
                            }
                        }

                        var nouveauBlock = context.Blocks.Find(blockActuelId.Value);
                        if (nouveauBlock != null)
                        {
                            nouveauBlock.EstOccupe = true;
                            nouveauBlock.TrainActuelId = train.Id;
                            train.BlockActuelId = blockActuelId.Value;
                        }

                        context.SaveChanges();
                    }

                    var trainFeature = new PointFeature(position)
                    {
                        ["TrainId"] = train.Id.ToString(),
                        ["Numero"] = train.Numero
                    };
                    newTrainFeatures.Add(trainFeature);
                    _trainPositionIndices[train.Id] = indexActuel + 2;
                }

                _trainsLayer.Features = newTrainFeatures;
                _trainsLayer.DataHasChanged();
                Map?.RefreshData();
            }
        }

        private int? TrouverBlockParPosition(MPoint position, ApplicationDbContext context)
        {
            var (lon, lat) = SphericalMercator.ToLonLat(position.X, position.Y);

            var tousLesBlocks = context.Blocks.ToList();

            foreach (var block in tousLesBlocks)
            {
                double distanceDebut = CalculerDistance(lat, lon, block.LatitudeDebut, block.LongitudeDebut);
                double distanceFin = CalculerDistance(lat, lon, block.LatitudeFin, block.LongitudeFin);

                if (distanceDebut < 200 || distanceFin < 200)
                {
                    return block.Id;
                }

                if (EstSurLigneBlock(lat, lon, block))
                {
                    return block.Id;
                }
            }

            return null;
        }

        private double CalculerDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; 
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private bool EstSurLigneBlock(double lat, double lon, Block block)
        {
            double tolerance = 0.002; 

            double minLat = Math.Min(block.LatitudeDebut, block.LatitudeFin) - tolerance;
            double maxLat = Math.Max(block.LatitudeDebut, block.LatitudeFin) + tolerance;
            double minLon = Math.Min(block.LongitudeDebut, block.LongitudeFin) - tolerance;
            double maxLon = Math.Max(block.LongitudeDebut, block.LongitudeFin) + tolerance;

            return lat >= minLat && lat <= maxLat && lon >= minLon && lon <= maxLon;
        }

        private void ZoomMap()
        {
            Map.Navigator.OverrideZoomBounds = new MMinMax(10, 500);

            double lon = -71.2080;
            double lat = 46.8139;

            var (x, y) = SphericalMercator.FromLonLat(lon, lat);
            var quebec = new MPoint(x, y);

            Map.Navigator.CenterOn(quebec);
            Map.Navigator.ZoomTo(170);
        }

        public Station GetStationById(int stationId)
        {
            return _stationDAL.GetById(stationId);
        }
    }
}