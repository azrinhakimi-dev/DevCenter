using DevCenter.Repositories;
using DevCenter.Services;
using DevCenter.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DevCenter
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddDbContext<AppDbContext>();

            services.AddTransient<HomeViewModel>();
            services.AddTransient<SnippetViewModel>();
            services.AddTransient<CommandViewModel>();
            services.AddTransient<CommandFormViewModel>();
            services.AddTransient<SnippetFormViewModel>();

            services.AddScoped<DevCommandRepo>();
            services.AddScoped<SnippetRepo>();


            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}