using Microsoft.Extensions.DependencyInjection;
using PoliceReport.Core.Action;
using PoliceReport.Core.Effectif;
using PoliceReport.Core.Grade;
using PoliceReport.Core.Infraction;
using PoliceReport.Core.Specialisation;
using PoliceReport.Core.Unite;
using PoliceReport.Core.Utilisateur;
using PoliceReport.Core.Vehicule;
using PoliceReport.Database;
using PoliceReport.Database.Dao;
using PoliceReport.Manager;

namespace PoliceReport
{
    public class Startup
    {
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Enregistrer la connexion à la base de données en tant que singleton
            services.AddSingleton<IDatabaseConnection, LocalDao>(provider => LocalDao.Instance);

            // Enregistrer les DAO
            services.AddTransient<IActionDao, ActionsDao>();
            services.AddTransient<IEffectifDao, EffectifsDao>();
            services.AddTransient<IGradeDao, GradesDao>();
            services.AddTransient<IInfractionDao, InfractionsDao>();
            services.AddTransient<ISpecialisationDao, SpecialisationsDao>();
            services.AddTransient<IUniteDao, UnitesDao>();
            services.AddTransient<IUtilisateurDao, UtilisateursDao>();
            services.AddTransient<IVehiculeDao, VehiculesDao>();

            // Enregistrer les managers
            services.AddSingleton<ITableManager, TableManager>();

            return services.BuildServiceProvider();
        }
    }
}
