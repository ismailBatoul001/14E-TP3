using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface INavigationService
    {
        BaseViewModel CurrentView { get; set; }
        void NavigateTo<T>() where T : BaseViewModel;
    }
}
