using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class MecanicienViewModel : BaseViewModel
    {
        private readonly IUserSessionService _session;
        private readonly IStationService _stationService;
        private readonly IInspectionService _inspectionService;

        public MecanicienViewModel(
            IUserSessionService session,
            IStationService stationService,
            IInspectionService inspectionService)
        {
            _session = session;
            _stationService = stationService;
            _inspectionService = inspectionService;

            RefreshTrainsCommand = new RelayCommand(ChargerTrains, CanAccess);
            RefreshInspectionsCommand = new RelayCommand(ChargerInspections, () => CanAccess() && SelectedTrain != null);
            CreerInspectionCommand = new RelayCommand(CreerInspection, () => CanAccess() && SelectedTrain != null);

            TypeSelectionne = TypeInspection.Visuelle;
            ResultatSelectionne = ResultatInspection.Conforme;

            ChargerTrains();
        }

        private bool CanAccess()
            => _session.ConnectedUser != null && _session.ConnectedUser.Role == Role.Mecanicien;

        public string Titre
            => _session.ConnectedUser == null
                ? "Espace mécanicien"
                : $"Espace mécanicien - {_session.ConnectedUser.Prenom} {_session.ConnectedUser.Nom}";

        private string? _message;
        public string? Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Train> Trains { get; } = new();
        public ObservableCollection<Inspection> Inspections { get; } = new();

        private Train? _selectedTrain;
        public Train? SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                _selectedTrain = value;
                OnPropertyChanged();

                ChargerInspections();

                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public Array TypesInspection => Enum.GetValues(typeof(TypeInspection));
        public Array ResultatsInspection => Enum.GetValues(typeof(ResultatInspection));

        private TypeInspection _typeSelectionne;
        public TypeInspection TypeSelectionne
        {
            get => _typeSelectionne;
            set { _typeSelectionne = value; OnPropertyChanged(); }
        }

        private ResultatInspection _resultatSelectionne;
        public ResultatInspection ResultatSelectionne
        {
            get => _resultatSelectionne;
            set { _resultatSelectionne = value; OnPropertyChanged(); }
        }

        private string _observations = "";
        public string Observations
        {
            get => _observations;
            set { _observations = value; OnPropertyChanged(); }
        }

        public ICommand RefreshTrainsCommand { get; }
        public ICommand RefreshInspectionsCommand { get; }
        public ICommand CreerInspectionCommand { get; }

        private void ChargerTrains()
        {
            Trains.Clear();
            Inspections.Clear();
            SelectedTrain = null;

            if (!CanAccess())
            {
                Message = "Accès refusé : vous devez être connecté en tant que mécanicien.";
                return;
            }

            var stationId = _session.ConnectedUser!.StationAssigneeId;

            var trains = _stationService.GetTrainsEnGare(stationId).ToList();

            foreach (var t in trains)
                Trains.Add(t);

            Message = trains.Count == 0
                ? "Aucun train en gare pour votre station."
                : $"{trains.Count} train(s) chargés.";
        }

        private void ChargerInspections()
        {
            Inspections.Clear();

            if (!CanAccess() || SelectedTrain == null)
                return;

            var list = _inspectionService.GetByTrainId(SelectedTrain.Id).OrderByDescending(i => i.DateInspection).ToList();

            foreach (var i in list)
                Inspections.Add(i);
        }

        private void CreerInspection()
        {
            if (!CanAccess() || SelectedTrain == null)
                return;

            try
            {
                var mecanicienId = _session.ConnectedUser!.Id;

                _inspectionService.CreerInspection(
                    trainId: SelectedTrain.Id,
                    mecanicienId: mecanicienId,
                    type: TypeSelectionne,
                    resultat: ResultatSelectionne,
                    observations: Observations
                );

                Observations = "";
                ChargerInspections();
                Message = "Inspection créée.";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}
