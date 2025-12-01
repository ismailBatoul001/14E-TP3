using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class PlanificationItineraireViewModel : BaseViewModel
    {
        private readonly IPlanificationItineraireService _planificationService;

        private ObservableCollection<Train> _trains;
        public ObservableCollection<Train> Trains
        {
            get => _trains;
            set
            {
                _trains = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Station> _stations;
        public ObservableCollection<Station> Stations
        {
            get => _stations;
            set
            {
                _stations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PointInteret> _pointsInteret;
        public ObservableCollection<PointInteret> PointsInteret
        {
            get => _pointsInteret;
            set
            {
                _pointsInteret = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<object> _arretsSelectionnes;
        public ObservableCollection<object> ArretsSelectionnes
        {
            get => _arretsSelectionnes;
            set
            {
                _arretsSelectionnes = value;
                OnPropertyChanged();
            }
        }

        private Train? _trainSelectionne;
        public Train? TrainSelectionne
        {
            get => _trainSelectionne;
            set
            {
                _trainSelectionne = value;
                OnPropertyChanged();
            }
        }

        private Station? _stationDepart;
        public Station? StationDepart
        {
            get => _stationDepart;
            set
            {
                _stationDepart = value;
                OnPropertyChanged();
            }
        }

        private Station? _stationArrivee;
        public Station? StationArrivee
        {
            get => _stationArrivee;
            set
            {
                _stationArrivee = value;
                OnPropertyChanged();
            }
        }

        private object? _elementSelectionne;
        public object? ElementSelectionne
        {
            get => _elementSelectionne;
            set
            {
                _elementSelectionne = value;
                OnPropertyChanged();
            }
        }

        private Itineraire? _itineraireActuel;
        public Itineraire? ItineraireActuel
        {
            get => _itineraireActuel;
            set
            {
                _itineraireActuel = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreerItineraireCommand { get; }
        public ICommand AjouterArretCommand { get; }
        public ICommand RetirerArretCommand { get; }
        public ICommand DemarrerItineraireCommand { get; }

        public PlanificationItineraireViewModel(IPlanificationItineraireService planificationService)
        {
            _planificationService = planificationService;
            ArretsSelectionnes = new ObservableCollection<object>();

            ChargerDonnees();

            CreerItineraireCommand = new RelayCommand(CreerItineraire, PeutCreerItineraire);
            AjouterArretCommand = new RelayCommand(AjouterArret, PeutAjouterArret);
            RetirerArretCommand = new RelayCommand(RetirerArret, PeutRetirerArret);
            DemarrerItineraireCommand = new RelayCommand(DemarrerItineraire, PeutDemarrerItineraire);
        }

        private void ChargerDonnees()
        {
            var trains = _planificationService.ObtenirTrainsDisponibles();
            Trains = new ObservableCollection<Train>(trains);

            var stations = _planificationService.ObtenirToutesLesStations();
            Stations = new ObservableCollection<Station>(stations);

            var points = _planificationService.ObtenirTousLesPointsInteret();
            PointsInteret = new ObservableCollection<PointInteret>(points);
        }

        private void CreerItineraire()
        {
            ClearErrors(nameof(TrainSelectionne));
            ClearErrors(nameof(StationDepart));
            ClearErrors(nameof(StationArrivee));

            if (!_planificationService.ValiderItineraire(
                TrainSelectionne,
                StationDepart,
                StationArrivee,
                out string erreur))
            {
                AddError(nameof(TrainSelectionne), erreur);
                OnPropertyChanged(nameof(ErrorMessages));
                return;
            }

            var arrets = _planificationService.ConstruireListeArrets(ArretsSelectionnes);

            ItineraireActuel = _planificationService.CreerNouvelItineraire(
                TrainSelectionne,
                StationDepart.Id,
                StationArrivee.Id,
                arrets
            );

            ClearErrors(nameof(TrainSelectionne));
            ArretsSelectionnes.Clear();
            TrainSelectionne = null;
            StationDepart = null;
            StationArrivee = null;

            ChargerDonnees();
        }

        private bool PeutCreerItineraire()
        {
            return TrainSelectionne != null &&
                   StationDepart != null &&
                   StationArrivee != null &&
                   !HasErrors;
        }

        private void AjouterArret()
        {
            if (ElementSelectionne != null && !ArretsSelectionnes.Contains(ElementSelectionne))
            {
                ArretsSelectionnes.Add(ElementSelectionne);
            }
        }

        private bool PeutAjouterArret()
        {
            return ElementSelectionne != null;
        }

        private void RetirerArret()
        {
            if (ElementSelectionne != null && ArretsSelectionnes.Contains(ElementSelectionne))
            {
                ArretsSelectionnes.Remove(ElementSelectionne);
            }
        }

        private bool PeutRetirerArret()
        {
            return ElementSelectionne != null && ArretsSelectionnes.Contains(ElementSelectionne);
        }

        private void DemarrerItineraire()
        {
            if (ItineraireActuel == null)
                return;

            _planificationService.DemarrerItineraire(ItineraireActuel.Id);

            ChargerDonnees();
            ItineraireActuel = null;
        }

        private bool PeutDemarrerItineraire()
        {
            return ItineraireActuel != null;
        }
    }
}