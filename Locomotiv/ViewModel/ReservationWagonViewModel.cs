using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class ReservationWagonViewModel : BaseViewModel
    {
        private readonly IReservationWagonService _reservationService;
        private readonly IStationService _stationService;
        private readonly IPlanificationItineraireService _itineraireService;
        private readonly IUserSessionService _userSession;

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

        private Station? _stationSelectionnee;
        public Station? StationSelectionnee
        {
            get => _stationSelectionnee;
            set
            {
                _stationSelectionnee = value;
                OnPropertyChanged();
                ChargerTrainsDisponibles();
            }
        }

        private ObservableCollection<Train> _trainsDisponibles;
        public ObservableCollection<Train> TrainsDisponibles
        {
            get => _trainsDisponibles;
            set
            {
                _trainsDisponibles = value;
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
                OnPropertyChanged(nameof(InfoTrain));
                CalculerTarif();
            }
        }

        private TypeWagon _typeWagonSelectionne;
        public TypeWagon TypeWagonSelectionne
        {
            get => _typeWagonSelectionne;
            set
            {
                _typeWagonSelectionne = value;
                OnPropertyChanged();
                CalculerTarif();
            }
        }

        private int _nombreWagons = 1;
        public int NombreWagons
        {
            get => _nombreWagons;
            set
            {
                _nombreWagons = value;
                OnPropertyChanged();
                CalculerTarif();
            }
        }

        private double _poids;
        public double Poids
        {
            get => _poids;
            set
            {
                _poids = value;
                OnPropertyChanged();
                CalculerTarif();
            }
        }

        private string? _notesSpeciales;
        public string? NotesSpeciales
        {
            get => _notesSpeciales;
            set
            {
                _notesSpeciales = value;
                OnPropertyChanged();
            }
        }

        private double _tarifEstime;
        public double TarifEstime
        {
            get => _tarifEstime;
            set
            {
                _tarifEstime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TarifEstimeFormate));
            }
        }

        public string TarifEstimeFormate => $"{TarifEstime:C2}";

        public string InfoTrain
        {
            get
            {
                if (TrainSelectionne == null)
                    return "Aucun train sélectionné";

                return $"Train {TrainSelectionne.Numero} - " +
                       $"{TrainSelectionne.NombreWagonsDisponibles}/{TrainSelectionne.NombreWagonsTotal} wagons disponibles - " +
                       $"Capacité: {TrainSelectionne.CapaciteChargeTonnes:F0} tonnes";
            }
        }

        private ObservableCollection<ReservationWagon> _mesReservations;
        public ObservableCollection<ReservationWagon> MesReservations
        {
            get => _mesReservations;
            set
            {
                _mesReservations = value;
                OnPropertyChanged();
            }
        }

        private ReservationWagon? _reservationSelectionnee;
        public ReservationWagon? ReservationSelectionnee
        {
            get => _reservationSelectionnee;
            set
            {
                _reservationSelectionnee = value;
                OnPropertyChanged();
            }
        }

        private string _messageStatut = string.Empty;
        public string MessageStatut
        {
            get => _messageStatut;
            set
            {
                _messageStatut = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReserverCommand { get; }
        public ICommand AnnulerReservationCommand { get; }
        public ICommand ActualiserCommand { get; }

        public ReservationWagonViewModel(
            IReservationWagonService reservationService,
            IStationService stationService,
            IPlanificationItineraireService itineraireService,
            IUserSessionService userSession)
        {
            _reservationService = reservationService;
            _stationService = stationService;
            _itineraireService = itineraireService;
            _userSession = userSession;

            TrainsDisponibles = new ObservableCollection<Train>();
            MesReservations = new ObservableCollection<ReservationWagon>();

            ReserverCommand = new RelayCommand(ReserverWagons, PeutReserver);
            AnnulerReservationCommand = new RelayCommand(AnnulerReservation, PeutAnnuler);
            ActualiserCommand = new RelayCommand(Actualiser);

            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            var stations = _stationService.GetAll();
            Stations = new ObservableCollection<Station>(stations);

            if (_userSession.ConnectedUser != null)
            {
                ChargerMesReservations();
            }
        }

        private void ChargerTrainsDisponibles()
        {
            if (StationSelectionnee == null)
            {
                TrainsDisponibles.Clear();
                return;
            }

            var trains = _reservationService.ObtenirTrainsCommerciaux(StationSelectionnee.Id);
            TrainsDisponibles = new ObservableCollection<Train>(trains);
        }

        private void ChargerMesReservations()
        {
            if (_userSession.ConnectedUser == null)
                return;

            var reservations = _reservationService.ObtenirReservationsClient(_userSession.ConnectedUser.Id);
            MesReservations = new ObservableCollection<ReservationWagon>(reservations);
        }

        private void CalculerTarif()
        {
            if (TrainSelectionne == null || NombreWagons <= 0 || Poids <= 0)
            {
                TarifEstime = 0;
                return;
            }

            TarifEstime = _reservationService.CalculerTarif(TypeWagonSelectionne, Poids, NombreWagons);
        }

        private void ReserverWagons()
        {
            try
            {
                if (_userSession.ConnectedUser == null)
                {
                    MessageStatut = "Vous devez être connecté pour faire une réservation.";
                    return;
                }

                if (TrainSelectionne == null)
                {
                    MessageStatut = "Veuillez sélectionner un train.";
                    return;
                }

                if (NombreWagons <= 0)
                {
                    MessageStatut = " Le nombre de wagons doit être supérieur à 0.";
                    return;
                }

                if (Poids <= 0)
                {
                    MessageStatut = "Le poids doit être supérieur à 0.";
                    return;
                }

                var stationDepart = StationSelectionnee;
                var stationArrivee = Stations.FirstOrDefault(s => s.Id != stationDepart?.Id);

                if (stationArrivee == null)
                {
                    MessageStatut = "Impossible de déterminer la destination.";
                    return;
                }

                var itineraire = _itineraireService.CreerNouvelItineraire(
                    TrainSelectionne,
                    stationDepart!.Id,
                    stationArrivee.Id,
                    new List<ItineraireArret>()
                );

                var reservation = _reservationService.CreerReservation(
                    _userSession.ConnectedUser.Id,
                    itineraire.Id,
                    NombreWagons,
                    TypeWagonSelectionne,
                    Poids,
                    NotesSpeciales
                );

                NombreWagons = 1;
                Poids = 0;
                NotesSpeciales = null;

                ChargerTrainsDisponibles();
                ChargerMesReservations();

                MessageStatut = $"Réservation créée avec succès! Numéro: {reservation.Id}";
            }
            catch (Exception ex)
            {
                MessageStatut = $"Erreur: {ex.Message}";
            }
        }

        private bool PeutReserver()
        {
            return TrainSelectionne != null &&
                   NombreWagons > 0 &&
                   Poids > 0 &&
                   _userSession.ConnectedUser != null;
        }

        private void AnnulerReservation()
        {
            if (ReservationSelectionnee == null)
                return;

            try
            {
                _reservationService.AnnulerReservation(ReservationSelectionnee.Id);
                ChargerMesReservations();
                ChargerTrainsDisponibles();

                MessageStatut = "Réservation annulée avec succès.";
            }
            catch (Exception ex)
            {
                MessageStatut = $"Erreur: {ex.Message}";
            }
        }

        private bool PeutAnnuler()
        {
            return ReservationSelectionnee != null &&
                   ReservationSelectionnee.Statut != StatutReservation.Completee &&
                   ReservationSelectionnee.Statut != StatutReservation.Annulee;
        }

        private void Actualiser()
        {
            ChargerDonnees();
            if (StationSelectionnee != null)
                ChargerTrainsDisponibles();
        }
    }
}