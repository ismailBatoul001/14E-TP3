using Locomotiv.ViewModel;

using Mapsui;

using Mapsui.UI;

using System.Windows;

using System.Windows.Controls;

using Locomotiv.Utils.Services;
using Locomotiv.ViewModel;

namespace Locomotiv.View

{

    public partial class MapView : UserControl

    {


        private MapViewModel _viewModel;

        public MapView()

        {

            InitializeComponent();


            this.Loaded += OnLoaded;
            this.Unloaded += MapView_Unloaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as MapViewModel;

            if (_viewModel == null)
                return;

            mapView.Map = _viewModel.Map;

            mapView.Info -= MapView_Info;
            mapView.Info += MapView_Info;
        }

        private void MapView_Info(object sender, MapInfoEventArgs e)

        {
            if (_viewModel?.Map == null)
                return;

            var stationsLayer = _viewModel.Map.Layers.FirstOrDefault(l => l.Name == "Gares");

            if (stationsLayer == null) return;

            var infoStation = e.GetMapInfo(new[] { stationsLayer });

            var stationTrouvee = infoStation?.Feature;

            if (stationTrouvee == null) return;

            object idObjet = stationTrouvee["Id"];

            if (idObjet == null) return;

            if (!int.TryParse(idObjet.ToString(), out int idStation)) return;

            var stationData = _viewModel.GetStationById(idStation);

            if (stationData == null)

            {

                MessageBox.Show("Erreur : station introuvable !");

                return;

            }

            var stationService = new StationService(
                   new Model.DAL.StationDAL(new ApplicationDbContext())
            );

            var detailsVM = new StationDetailsViewModel(
                stationService,
                idStation

            );
            var detailsVue = new StationDetailsView

            {

                DataContext = detailsVM

            };

            PopupContent.Content = detailsVue;

            StationPopup.Visibility = Visibility.Visible;

        }

        private void MapView_Unloaded(object sender, RoutedEventArgs e)

        {

            _viewModel?.ArreterTimer();

        }

        private void FermerPopup_Click(object sender, RoutedEventArgs e)

        {

            StationPopup.Visibility = Visibility.Collapsed;

            PopupContent.Content = null;

        }

    }

}