using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;

namespace Locomotiv.ViewModel
{
    class ConnectUserViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private readonly IStationService _stationService;
        private INavigationService _navigationService;
        private IUserSessionService _userSessionService;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                    ValidateProperty(nameof(Username), value);
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                    ValidateProperty(nameof(Password), value);
                }
            }
        }

        public ConnectUserViewModel(IUserDAL userDAL, IStationService stationService, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userDAL = userDAL;
            _stationService = stationService;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            ConnectCommand = new RelayCommand(Connect, CanConnect);
        }

        public ICommand ConnectCommand { get; set; }

        private void Connect()
        {
            User? user = _userDAL.FindByUsernameAndPassword(Username, Password);
            if (user != null)
            {
                _userSessionService.ConnectedUser = user;

                if (user.Role == Role.Employe && user.StationAssigneeId != 0)
                {
                    var stationDetailsVM = new StationDetailsViewModel(_stationService, user.StationAssigneeId);
                    _navigationService.CurrentView = stationDetailsVM;
                }
                else if (user.Role == Role.Administrateur)
                {
                    _navigationService.NavigateTo<HomeViewModel>();
                }
            }
            else
            {
                AddError(nameof(Password), "Utilisateur ou mot de passe invalide.");
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        private bool CanConnect()
        {
            bool allRequiredFieldsAreEntered = Username.NotEmpty() && Password.NotEmpty();
            return !HasErrors && allRequiredFieldsAreEntered;
        }

        private void ValidateProperty(string propertyName, string value)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(Username):
                    if (value.Empty())
                    {
                        AddError(propertyName, "Le nom d'utilisateur est requis.");
                    }
                    else if (value.Length < 2)
                    {
                        AddError(propertyName, "Le nom d'utilisateur doit contenir au moins 2 caractères.");
                    }
                    break;
                case nameof(Password):
                    if (value.Empty())
                    {
                        AddError(propertyName, "Le mot de passe est requis.");
                    }
                    break;
            }
            OnPropertyChanged(nameof(ErrorMessages));
        }
    }
}
