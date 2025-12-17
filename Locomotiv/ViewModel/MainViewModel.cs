using System.Windows.Input;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;

namespace Locomotiv.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;
        private readonly IStationService _stationService;

        public INavigationService NavigationService => _navigationService;
        public IUserSessionService UserSessionService => _userSessionService;

        public ICommand NavigateToReservationItineraireCommand { get; }
        public ICommand NavigateToGestionTrainCommand { get; }
        public ICommand NavigateToConnectUserViewCommand { get; }
        public ICommand NavigateToHomeViewCommand { get; }
        public ICommand NavigateToMapViewCommand { get; }
        public ICommand NavigateToPlanificationCommand { get; }
        public ICommand DisconnectCommand { get; }

        public MainViewModel(INavigationService navigationService,
                           IUserSessionService userSessionService,
                           IStationService stationService)
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            _stationService = stationService;

            NavigateToConnectUserViewCommand = new RelayCommand(() => NavigationService.NavigateTo<ConnectUserViewModel>());

            NavigateToHomeViewCommand = new RelayCommand(NavigateToHome);

            DisconnectCommand = new RelayCommand(Disconnect, () => UserSessionService.IsUserConnected);

            NavigateToGestionTrainCommand = new RelayCommand(
                () => NavigationService.NavigateTo<GestionTrainViewModel>(),
                IsUserAdmin);

            NavigateToMapViewCommand = new RelayCommand(
                () => NavigationService.NavigateTo<MapViewModel>(),
                IsUserAdmin);

            NavigateToPlanificationCommand = new RelayCommand(
                () => NavigationService.NavigateTo<PlanificationItineraireViewModel>(),
                IsUserAdmin);
            NavigateToReservationItineraireCommand = new RelayCommand(
                () => NavigationService.NavigateTo<ReservationItineraireViewModel>());

            NavigationService.NavigateTo<HomeViewModel>();
        }

        private void NavigateToHome()
        {
            var user = _userSessionService.ConnectedUser;

            if (user?.Role == Role.Employe && user.StationAssigneeId != 0)
            {
                var stationDetailsVM = new StationDetailsViewModel(_stationService, user.StationAssigneeId);
                _navigationService.CurrentView = stationDetailsVM;
            }
            else
            {
                _navigationService.NavigateTo<HomeViewModel>();
            }
        }

        private void Disconnect()
        {
            _userSessionService.ConnectedUser = null;
            OnPropertyChanged(nameof(UserSessionService.IsUserConnected));
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        private bool IsUserAdmin()
        {
            var user = _userSessionService.ConnectedUser;
            return user != null && user.Role == Role.Administrateur;
        }
    }
}