using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using System.Collections.ObjectModel;
using Locomotiv.Utils;
using System.Windows.Input;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Mapsui.Extensions;

namespace Locomotiv.ViewModel
{
    public class GestionTrainViewModel : BaseViewModel
    {
        private readonly ITrainService _trainService;
        private readonly IStationService _stationService;

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

        private string _nouveauTrainNumero;
        public string NouveauTrainNumero
        {
            get => _nouveauTrainNumero;
            set
            {
                _nouveauTrainNumero = value;
                OnPropertyChanged();
                ValidateProprety(nameof(NouveauTrainNumero), value);
            }
        }

        private TypeTrain _nouveauTrainType;
        public TypeTrain NouveauTrainType
        {
            get => _nouveauTrainType;
            set
            {
                _nouveauTrainType = value;
                OnPropertyChanged();
            }
        }

        private EtatTrain _nouveauTrainEtat;
        public EtatTrain NouveauTrainEtat
        {
            get => _nouveauTrainEtat;
            set
            {
                _nouveauTrainEtat = value;
                OnPropertyChanged();
            }
        }

        private int _nouveauTrainCapacite;
        public int NouveauTrainCapacite
        {
            get => _nouveauTrainCapacite;
            set
            {
                _nouveauTrainCapacite = value;
                OnPropertyChanged();
                ValidateProprety(nameof(NouveauTrainCapacite), value.ToString());
            }
        }

        private Train? _selectedTrain;
        public Train? SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                _selectedTrain = value;
                OnPropertyChanged();
            }
        }

        private Station? _selectedStation;
        public Station? SelectedStation
        {
            get => _selectedStation;
            set
            {
                _selectedStation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CapaciteStation));
            }
        }

        public string CapaciteStation
        {
            get
            {
                if (SelectedStation == null)
                {
                    return "";
                }
                int nbTrains = _trainService.CompterTrainsStation(SelectedStation.Id);
                return $"Capacité: {nbTrains} sur {SelectedStation.CapaciteMaximale}";
            }
        }

        public ICommand AjouterTrainCommand { get; set; }
        public ICommand SupprimerTrainCommand { get; set; }

        public GestionTrainViewModel(ITrainService trainService, IStationService stationService)
        {
            _trainService = trainService;
            _stationService = stationService;

            ChargerTrains();
            ChargerStations();

            AjouterTrainCommand = new RelayCommand(AjouterTrain, CanAjouterTrain);
            SupprimerTrainCommand = new RelayCommand(SupprimerTrain, CanSupprimerTrain);
        }

        private void ChargerTrains()
        {
            var trains = _trainService.GetAll();
            Trains = new ObservableCollection<Train>(trains);
        }

        private void ChargerStations()
        {
            IEnumerable<Station> stations;
            try
            {
                stations = _stationService.GetAll();
                Stations = new ObservableCollection<Station>(stations);
            }
            catch (Exception ex)
            {
                AddError(nameof(stations), ex.Message);
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        private void AjouterTrain()
        {
            try
            {
                _trainService.AjouterTrain(
                    NouveauTrainNumero,
                    NouveauTrainType,
                    NouveauTrainEtat,
                    NouveauTrainCapacite,
                    SelectedStation.Id
                );
            }
            catch(Exception ex)
            {
                AddError(nameof(NouveauTrainNumero), ex.Message);
                OnPropertyChanged(nameof(ErrorMessages));
            }

            ChargerTrains();
        }

        private bool CanAjouterTrain()
        {
            return !string.IsNullOrWhiteSpace(NouveauTrainNumero) && 
                NouveauTrainCapacite > 0 && 
                SelectedStation != null && 
                !HasErrors;
        }

        private void SupprimerTrain()
        {
            try
            {
                _trainService.SupprimerTrain(SelectedTrain);
            }
            catch (Exception ex)
            {
                AddError(nameof(SelectedTrain), ex.Message);
                OnPropertyChanged(nameof(ErrorMessages));
            }

            ChargerTrains();
            ViderFormulaire();
        }

        private void ViderFormulaire()
        {
            NouveauTrainNumero = string.Empty;
            NouveauTrainType = TypeTrain.Passagers;
            NouveauTrainEtat = EtatTrain.Programme;
            NouveauTrainCapacite = 0;
            SelectedTrain = null;
        }

        private bool CanSupprimerTrain()
        {
            return SelectedTrain != null;
        }

        private void ValidateProprety(string propertyName, string value)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(NouveauTrainNumero):
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        AddError(propertyName, "Le numéro du train est requis.");
                    }
                    else if (value.Length < 3)
                    {
                        AddError(propertyName, "Le numéro du train doit contenir au moins 3 caractères.");
                    }
                    break;

                case nameof(NouveauTrainCapacite):
                    if (!int.TryParse(value, out int capacite) || capacite <= 0)
                    {
                        AddError(propertyName, "La capacité du train doit être un nombre positif.");
                    }
                    break;
            }
            OnPropertyChanged(nameof(ErrorMessages));
        }
    }
}
