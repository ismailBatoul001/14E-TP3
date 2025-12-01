using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Locomotiv.Model;

namespace Locomotiv.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;

        public User? ConnectedUser
        {
            get => _userSessionService.ConnectedUser;
        }

        public string WelcomeMessage
        {
            get
            {
                if (ConnectedUser == null)
                    return "Bienvenue! Veuillez vous connecter.";

                if (ConnectedUser.Role == Role.Administrateur)
                    return $"Bienvenue {ConnectedUser.Nom}!";

                return $"Bienvenue {ConnectedUser.Prenom} {ConnectedUser.Nom}!";
            }
        }


        public HomeViewModel(IUserDAL userDAL, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

        public ICommand LogoutCommand { get; set; }

        private void Logout()
        {
            _userSessionService.ConnectedUser = null;
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }
    }
}
