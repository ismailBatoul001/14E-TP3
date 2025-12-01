using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using System.Collections.ObjectModel;
using Locomotiv.Utils.Services.Interfaces;

namespace Locomotiv.ViewModel
{
    public class StationDetailsViewModel : BaseViewModel
    {
        private readonly IStationService _stationService;
        private Station? _station;

        public Station? Station
        {
            get => _station;
            set
            {
                _station = value;
                OnPropertyChanged();
                ChargerDetailStation();
            }
        }

        public ObservableCollection<Voie> Voies { get; set; }
        public ObservableCollection<Signal> Signaux { get; set; }
        public ObservableCollection<Train> TrainsEnGare { get; set; }

        public string StationNom
        {
            get
            {
                if (Station == null)
                    return "Aucune station";
                return _stationService.GetNomStation(Station.Id);
            }
        }

        public string StationPosition
        {
            get
            {
                if(Station == null)
                    return "Position inconnue, car aucune station détectée";
                return _stationService.GetPositionStation(Station.Id);
            }
        }

        public string StationCapacite
        {
            get
            {
                if(Station == null)
                    return "Capacité inconnue, car aucune station détectée";
                return _stationService.GetCapaciteStation(Station.Id);
            }
        }

        public StationDetailsViewModel(IStationService stationService, int stationId)
        {
            _stationService = stationService;
            Voies = new ObservableCollection<Voie>();
            Signaux = new ObservableCollection<Signal>();
            TrainsEnGare = new ObservableCollection<Train>();
            ChargerStation(stationId);
        }

        private void ChargerStation(int stationId)
        {
            Station = _stationService.GetById(stationId);
        }

        private void ChargerDetailStation()
        {
            if (Station == null)
                return;

            Voies.Clear();
            foreach (var voie in _stationService.GetVoies(Station.Id))
            {
                Voies.Add(voie);
            }
            Signaux.Clear();
            foreach (var signal in _stationService.GetSignaux(Station.Id))
            {
                Signaux.Add(signal);
            }
            TrainsEnGare.Clear();
            foreach (var trainEnGare in _stationService.GetTrainsEnGare(Station.Id))
            {
                TrainsEnGare.Add(trainEnGare);
            }
            OnPropertyChanged(nameof(StationNom));
            OnPropertyChanged(nameof(StationPosition));
            OnPropertyChanged(nameof(StationCapacite));
        }
    }
}