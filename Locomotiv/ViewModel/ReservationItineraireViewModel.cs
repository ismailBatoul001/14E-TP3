using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class ReservationItineraireViewModel : BaseViewModel
    {
        private readonly IItineraireService _itineraireService;
        private readonly IStationService _stationService;
        private readonly IReservationService _reservationService;
        private int _userId;

        #region Propriétés - Recherche d'itinéraires

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

        private ObservableCollection<ItineraireAffichage> _itinerairesDisponibles;
        public ObservableCollection<ItineraireAffichage> ItinerairesDisponibles
        {
            get => _itinerairesDisponibles;
            set
            {
                _itinerairesDisponibles = value;
                OnPropertyChanged();
            }
        }

        private Station _stationDepartSelectionnee;
        public Station StationDepartSelectionnee
        {
            get => _stationDepartSelectionnee;
            set
            {
                _stationDepartSelectionnee = value;
                OnPropertyChanged();
            }
        }

        private Station _stationArriveeSelectionnee;
        public Station StationArriveeSelectionnee
        {
            get => _stationArriveeSelectionnee;
            set
            {
                _stationArriveeSelectionnee = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _dateDepartSelectionnee;
        public DateTime? DateDepartSelectionnee
        {
            get => _dateDepartSelectionnee;
            set
            {
                _dateDepartSelectionnee = value;
                OnPropertyChanged();
            }
        }

        private string _heureDepartSouhaitee;
        public string HeureDepartSouhaitee
        {
            get => _heureDepartSouhaitee;
            set
            {
                _heureDepartSouhaitee = value;
                OnPropertyChanged();
                ValidateHeure(value);
            }
        }

        private int _nombrePassagers = 1;
        public int NombrePassagers
        {
            get => _nombrePassagers;
            set
            {
                _nombrePassagers = value;
                OnPropertyChanged();
                ValidateNombrePassagers(value);
            }
        }

        private ItineraireAffichage _itineraireSelectionne;
        public ItineraireAffichage ItineraireSelectionne
        {
            get => _itineraireSelectionne;
            set
            {
                _itineraireSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DetailsItineraireSelectionne));
            }
        }

        public string DetailsItineraireSelectionne
        {
            get
            {
                if (ItineraireSelectionne == null)
                {
                    return "Aucun itinéraire sélectionné. Sélectionnez un itinéraire dans la liste pour voir les détails complets.";
                }

                decimal montantTotal = ItineraireSelectionne.TarifApproximatif * NombrePassagers;

                return $"Train: {ItineraireSelectionne.Train.Numero}\n" +
                       $"Type: {ItineraireSelectionne.Train.Type}\n" +
                       $"Départ: {ItineraireSelectionne.StationDepart.Nom} à {ItineraireSelectionne.HeureDepart:HH:mm}\n" +
                       $"Arrivée: {ItineraireSelectionne.StationArrivee.Nom} à {ItineraireSelectionne.HeureArrivee:HH:mm}\n" +
                       $"Durée estimée: {(ItineraireSelectionne.HeureArrivee - ItineraireSelectionne.HeureDepart).TotalMinutes:F0} minutes\n" +
                       $"Stations intermédiaires: {ItineraireSelectionne.StationsIntermediaires}\n" +
                       $"Places disponibles: {ItineraireSelectionne.PlacesDisponibles}/{ItineraireSelectionne.Train.Capacite}\n" +
                       $"Tarif par personne: {ItineraireSelectionne.TarifApproximatif:C}\n" +
                       $"Nombre de passagers: {NombrePassagers}\n" +
                       $"Montant total: {montantTotal:C}";
            }
        }

        #endregion

        #region Propriétés - Mes Réservations

        private ObservableCollection<Reservation> _mesReservations;
        public ObservableCollection<Reservation> MesReservations
        {
            get => _mesReservations;
            set
            {
                _mesReservations = value;
                OnPropertyChanged();
            }
        }

        private Reservation _reservationSelectionnee;
        public Reservation ReservationSelectionnee
        {
            get => _reservationSelectionnee;
            set
            {
                _reservationSelectionnee = value;
                OnPropertyChanged();
            }
        }

        private StatutReservation? _statutFiltre;
        public StatutReservation? StatutFiltre
        {
            get => _statutFiltre;
            set
            {
                _statutFiltre = value;
                OnPropertyChanged();
                ChargerReservations();
            }
        }

        #endregion

        #region Commandes

        public ICommand RechercherItinerairesCommand { get; set; }
        public ICommand ReserverItineraireCommand { get; set; }
        public ICommand RafraichirReservationsCommand { get; set; }
        public ICommand VoirDetailsReservationCommand { get; set; }
        public ICommand AnnulerReservationCommand { get; set; }

        #endregion

        public ReservationItineraireViewModel(
            IItineraireService itineraireService,
            IStationService stationService,
            IReservationService reservationService)
        {
            _itineraireService = itineraireService;
            _stationService = stationService;
            _reservationService = reservationService;

            // TODO: Récupérer l'ID de l'utilisateur connecté
            _userId = 1;

            ChargerStations();
            InitialiserCommandes();

            DateDepartSelectionnee = DateTime.Today;
            ItinerairesDisponibles = new ObservableCollection<ItineraireAffichage>();
            MesReservations = new ObservableCollection<Reservation>();

            ChargerReservations();
        }

        private void InitialiserCommandes()
        {
            RechercherItinerairesCommand = new RelayCommand(RechercherItineraires, CanRechercherItineraires);
            ReserverItineraireCommand = new RelayCommand(ReserverItineraire, CanReserverItineraire);
            RafraichirReservationsCommand = new RelayCommand(ChargerReservations);
            VoirDetailsReservationCommand = new RelayCommand(VoirDetailsReservation, CanVoirDetailsReservation);
            AnnulerReservationCommand = new RelayCommand(AnnulerReservation, CanAnnulerReservation);
        }

        #region Méthodes - Recherche d'itinéraires

        private void ChargerStations()
        {
            try
            {
                var stations = _stationService.GetAll();
                Stations = new ObservableCollection<Station>(stations);
            }
            catch (Exception ex)
            {
                AddError(nameof(Stations), ex.Message);
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        private void RechercherItineraires()
        {
            ClearErrors(nameof(StationDepartSelectionnee));
            ClearErrors(nameof(StationArriveeSelectionnee));

            try
            {
                int? stationDepartId = StationDepartSelectionnee?.Id;
                int? stationArriveeId = StationArriveeSelectionnee?.Id;
                DateTime? dateDepart = DateDepartSelectionnee;

                TimeSpan? heureDepartMin = null;
                if (!string.IsNullOrWhiteSpace(HeureDepartSouhaitee) &&
                    TimeSpan.TryParse(HeureDepartSouhaitee, out TimeSpan heure))
                {
                    heureDepartMin = heure;
                }

                var itineraires = _itineraireService.RechercherItineraires(
                    stationDepartId,
                    stationArriveeId,
                    dateDepart,
                    heureDepartMin);

                var itinerairesAffichage = itineraires
                    .Select(i => CreerItineraireAffichage(i))
                    .Where(i => i.PlacesDisponibles >= NombrePassagers)
                    .ToList();

                ItinerairesDisponibles = new ObservableCollection<ItineraireAffichage>(itinerairesAffichage);

                if (ItinerairesDisponibles.Count == 0)
                {
                    AddError(nameof(StationDepartSelectionnee), $"Aucun itinéraire disponible pour {NombrePassagers} passager(s) avec les critères sélectionnés.");
                }
            }
            catch (Exception ex)
            {
                AddError(nameof(StationDepartSelectionnee), $"Erreur lors de la recherche: {ex.Message}");
            }

            OnPropertyChanged(nameof(ErrorMessages));
        }

        private bool CanRechercherItineraires()
        {
            return StationDepartSelectionnee != null ||
                   StationArriveeSelectionnee != null ||
                   DateDepartSelectionnee.HasValue;
        }

        private void ReserverItineraire()
        {
            try
            {
                if (ItineraireSelectionne.PlacesDisponibles < NombrePassagers)
                {
                    AddError(nameof(ItineraireSelectionne), $"Pas assez de places disponibles. Places restantes: {ItineraireSelectionne.PlacesDisponibles}");
                    OnPropertyChanged(nameof(ErrorMessages));
                    return;
                }

                var reservation = _reservationService.CreerReservation(
                    ItineraireSelectionne.Itineraire.Id,
                    _userId,
                    NombrePassagers);

                System.Windows.MessageBox.Show(
                    $"Réservation confirmée !\n\n" +
                    $"Numéro de billet: {reservation.NumeroBillet}\n" +
                    $"Train: {ItineraireSelectionne.Train.Numero}\n" +
                    $"De {ItineraireSelectionne.StationDepart.Nom} à {ItineraireSelectionne.StationArrivee.Nom}\n" +
                    $"Départ: {ItineraireSelectionne.HeureDepart:dd/MM/yyyy à HH:mm}\n" +
                    $"Nombre de passagers: {reservation.NombrePassagers}\n" +
                    $"Montant total: {reservation.MontantTotal:C}\n\n" +
                    $"Conservez votre numéro de billet pour l'embarquement.",
                    "Réservation confirmée",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);

                RechercherItineraires();
                ChargerReservations();
            }
            catch (Exception ex)
            {
                AddError(nameof(ItineraireSelectionne), $"Erreur lors de la réservation: {ex.Message}");
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        private bool CanReserverItineraire()
        {
            return ItineraireSelectionne != null &&
                   ItineraireSelectionne.PlacesDisponibles >= NombrePassagers &&
                   NombrePassagers > 0;
        }

        private ItineraireAffichage CreerItineraireAffichage(Itineraire itineraire)
        {
            int placesDisponibles = _itineraireService.CalculerPlacesDisponibles(itineraire.Id);
            string stationsIntermediaires = ObtenirStationsIntermediaires(itineraire);
            decimal tarif = _itineraireService.CalculerTarifItineraire(itineraire.Id);
            DateTime heureDepart = itineraire.DateCreation;
            DateTime heureArrivee = CalculerHeureArrivee(itineraire);

            return new ItineraireAffichage
            {
                Itineraire = itineraire,
                Train = itineraire.Train,
                StationDepart = itineraire.StationDepart,
                StationArrivee = itineraire.StationArrivee,
                HeureDepart = heureDepart,
                HeureArrivee = heureArrivee,
                StationsIntermediaires = stationsIntermediaires,
                PlacesDisponibles = placesDisponibles,
                TarifApproximatif = tarif
            };
        }

        private string ObtenirStationsIntermediaires(Itineraire itineraire)
        {
            if (itineraire.Arrets == null || !itineraire.Arrets.Any())
            {
                return "Aucun arrêt intermédiaire";
            }

            var nomsStations = itineraire.Arrets
                .OrderBy(a => a.Ordre)
                .Select(a => a.Station?.Nom ?? "Station inconnue")
                .ToList();

            return string.Join(", ", nomsStations);
        }

        private DateTime CalculerHeureArrivee(Itineraire itineraire)
        {
            if (itineraire.Arrets != null && itineraire.Arrets.Any())
            {
                var dernierArret = itineraire.Arrets.OrderByDescending(a => a.Ordre).FirstOrDefault();
                if (dernierArret != null)
                {
                    return dernierArret.HeureDepart.AddMinutes(30);
                }
            }

            return itineraire.DateCreation.AddHours(2);
        }

        private void ValidateHeure(string heure)
        {
            ClearErrors(nameof(HeureDepartSouhaitee));

            if (string.IsNullOrWhiteSpace(heure))
            {
                return;
            }

            if (!TimeSpan.TryParse(heure, out _))
            {
                AddError(nameof(HeureDepartSouhaitee), "Format d'heure invalide. Utilisez le format HH:mm (ex: 14:30)");
            }

            OnPropertyChanged(nameof(ErrorMessages));
        }

        private void ValidateNombrePassagers(int nombre)
        {
            ClearErrors(nameof(NombrePassagers));

            if (nombre <= 0)
            {
                AddError(nameof(NombrePassagers), "Le nombre de passagers doit être au moins 1.");
            }
            else if (nombre > 10)
            {
                AddError(nameof(NombrePassagers), "Maximum 10 passagers par réservation.");
            }

            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(DetailsItineraireSelectionne));
        }

        #endregion

        #region Méthodes - Mes Réservations

        private void ChargerReservations()
        {
            try
            {
                var reservations = _reservationService.GetReservationsParUtilisateur(_userId);

                if (StatutFiltre.HasValue)
                {
                    reservations = reservations.Where(r => r.Statut == StatutFiltre.Value);
                }

                MesReservations = new ObservableCollection<Reservation>(reservations);
            }
            catch (Exception ex)
            {
                AddError(nameof(MesReservations), $"Erreur lors du chargement des réservations: {ex.Message}");
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        private void VoirDetailsReservation()
        {
            if (ReservationSelectionnee == null) return;

            string details = $"Détails de la réservation\n\n" +
                           $"Numéro de billet: {ReservationSelectionnee.NumeroBillet}\n" +
                           $"Statut: {ReservationSelectionnee.Statut}\n\n" +
                           $"Train: {ReservationSelectionnee.Itineraire.Train.Numero}\n" +
                           $"Type: {ReservationSelectionnee.Itineraire.Train.Type}\n\n" +
                           $"Départ: {ReservationSelectionnee.Itineraire.StationDepart.Nom}\n" +
                           $"Arrivée: {ReservationSelectionnee.Itineraire.StationArrivee.Nom}\n" +
                           $"Date de départ: {ReservationSelectionnee.Itineraire.DateCreation:dd/MM/yyyy à HH:mm}\n\n" +
                           $"Nombre de passagers: {ReservationSelectionnee.NombrePassagers}\n" +
                           $"Montant total: {ReservationSelectionnee.MontantTotal:C}\n" +
                           $"Date de réservation: {ReservationSelectionnee.DateReservation:dd/MM/yyyy à HH:mm}";

            if (ReservationSelectionnee.DateAnnulation.HasValue)
            {
                details += $"\n\nAnnulé le: {ReservationSelectionnee.DateAnnulation:dd/MM/yyyy à HH:mm}";
            }

            System.Windows.MessageBox.Show(
                details,
                "Détails de la réservation",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        private bool CanVoirDetailsReservation()
        {
            return ReservationSelectionnee != null;
        }

        private void AnnulerReservation()
        {
            if (ReservationSelectionnee == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Êtes-vous sûr de vouloir annuler cette réservation ?\n\n" +
                $"Billet: {ReservationSelectionnee.NumeroBillet}\n" +
                $"Train: {ReservationSelectionnee.Itineraire.Train.Numero}\n" +
                $"Départ: {ReservationSelectionnee.Itineraire.DateCreation:dd/MM/yyyy à HH:mm}\n\n" +
                $"Cette action est irréversible.",
                "Confirmation d'annulation",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    _reservationService.AnnulerReservation(ReservationSelectionnee.Id);

                    System.Windows.MessageBox.Show(
                        "Votre réservation a été annulée avec succès.",
                        "Annulation confirmée",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);

                    ChargerReservations();
                    RechercherItineraires();
                }
                catch (Exception ex)
                {
                    AddError(nameof(ReservationSelectionnee), $"Erreur lors de l'annulation: {ex.Message}");
                    OnPropertyChanged(nameof(ErrorMessages));
                }
            }
        }

        private bool CanAnnulerReservation()
        {
            return ReservationSelectionnee != null &&
                   ReservationSelectionnee.Statut != StatutReservation.Annulee &&
                   ReservationSelectionnee.Statut != StatutReservation.Terminee &&
                   ReservationSelectionnee.Itineraire.DateCreation > DateTime.Now;
        }

        #endregion
    }

    public class ItineraireAffichage
    {
        public Itineraire Itineraire { get; set; }
        public Train Train { get; set; }
        public Station StationDepart { get; set; }
        public Station StationArrivee { get; set; }
        public DateTime HeureDepart { get; set; }
        public DateTime HeureArrivee { get; set; }
        public string StationsIntermediaires { get; set; }
        public int PlacesDisponibles { get; set; }
        public decimal TarifApproximatif { get; set; }

        public string MessageDisponibilite
        {
            get
            {
                if (PlacesDisponibles == 0)
                {
                    return "COMPLET";
                }
                else if (PlacesDisponibles <= 5)
                {
                    return "Dernières places disponibles";
                }
                return "";
            }
        }

        public string CouleurDisponibilite
        {
            get
            {
                if (PlacesDisponibles == 0)
                {
                    return "Red";
                }
                else if (PlacesDisponibles <= 5)
                {
                    return "Orange";
                }
                return "Green";
            }
        }
    }
}