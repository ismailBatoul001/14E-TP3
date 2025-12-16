using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Locomotiv.ViewModel
{
    public class ReservationWagonViewModel : BaseViewModel
    {
        private readonly IReservationWagonService _reservationService;
        private readonly IStationService _stationService;
        private readonly IPlanificationItineraireService _itineraireService;
        private readonly IUserSessionService _userSession;

        private ObservableCollection<Station> _stations;
    }
}
