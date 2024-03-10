using APass.Core.Interfaces;
using APass.Core.Services;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using APass.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.IO;
using APass.Infrastructure.Repositories;
using APass.Core.Entities;


namespace APass.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PasswordManagerContext>();
            services.AddSingleton<ICryptographicManager, CryptographicManager>();
            services.AddSingleton<IRepository<PasswordEntry>, PasswordEntryRepository>();
            services.AddSingleton<IRepository<MasterPassword>, MasterPasswordRepository>();
            services.AddTransient<LoginWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeDatabase();
            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();
        }
        private void InitializeDatabase()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolderPath = Path.Combine(folderPath, "APass");
            var dbPath = Path.Combine(appFolderPath, "passwordManager.db");

            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }

            // Configure the DbContext to use the SQLite database
            var optionsBuilder = new DbContextOptionsBuilder<PasswordManagerContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            using (var context = new PasswordManagerContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
            }
        }
    }

}
