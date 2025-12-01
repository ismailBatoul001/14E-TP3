using System.Windows;
using Locomotiv.Utils;
using Locomotiv.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Locomotiv.Model.Interfaces;
using Locomotiv.Model.DAL;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Locomotiv
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>();

            services.AddScoped<IStationDAL, StationDAL>();
            services.AddScoped<IPointInteretDAL, PointInteretDAL>();
            services.AddScoped<IUserDAL, UserDAL>();
            services.AddScoped<BlockDAL>();
            services.AddScoped<ITrainRepository, TrainRepository>();

            services.AddScoped<IRailNetworkService, RailNetworkService>();
            services.AddScoped<IRailGeometryService, RailGeometryService>();
            services.AddScoped<IBlockGeometryService, BlockGeometryService>();
            services.AddScoped<ITrainPathService, TrainPathService>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserSessionService, Service>();
            services.AddScoped<IItineraireService, ItineraireService>();

            services.AddScoped<ITrainService, TrainService>();
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IPointInteretService, PointInteretService>();
            services.AddScoped<IBlockService, BlockService>();

            services.AddTransient<GestionTrainViewModel>();
            services.AddTransient<PlanificationItineraireViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<ConnectUserViewModel>();
            services.AddSingleton<StationDetailsViewModel>();
            services.AddTransient<MapViewModel>();
            
            services.AddScoped<IPlanificationItineraireService, PlanificationItineraireService>();

            services.AddSingleton<IRailGeometryService, RailGeometryService>();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
            {
                BaseViewModel ViewModelFactory(Type viewModelType)
                {
                    return (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
                }
                return ViewModelFactory;
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.Migrate();

                if (!dbContext.Stations.Any())
                {
                    dbContext.SeedData();
                }
            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}