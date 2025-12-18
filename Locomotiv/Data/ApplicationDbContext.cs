using Microsoft.EntityFrameworkCore;
using Locomotiv.Utils.Services;
using System.IO;
using Locomotiv.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnConfiguring(
       DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Locomotiv", "Locomotiv.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
            var connectionString = $"Data Source={dbPath}";

            optionsBuilder.UseSqlite(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Block>()
            .HasOne(b => b.TrainActuel)
            .WithOne(t => t.BlockActuel)
            .HasForeignKey<Train>(t => t.BlockActuelId);

        modelBuilder.Entity<Voie>()
            .HasOne(v => v.TrainActuel)
            .WithOne(t => t.VoieActuelle)
            .HasForeignKey<Train>(t => t.VoieActuelleId);

        modelBuilder.Entity<Station>()
            .HasMany(s => s.TrainsEnGare)
            .WithOne(t => t.StationActuelle)
            .HasForeignKey(t => t.StationActuelleId);

        modelBuilder.Entity<Station>()
            .HasMany(s => s.Voies)
            .WithOne(v => v.Station)
            .HasForeignKey(v => v.StationId);

        modelBuilder.Entity<Inspection>()
            .HasOne(i => i.Train)
            .WithMany(t => t.Inspections)
            .HasForeignKey(i => i.TrainId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Inspection>()
            .HasOne(i => i.Mecanicien)
            .WithMany(u => u.InspectionsEffectuees)
            .HasForeignKey(i => i.MecanicienId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItineraireArret>(entity =>
        {
            entity.HasOne(a => a.Station)
                .WithMany()
                .HasForeignKey(a => a.StationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(a => a.PointInteret)
                .WithMany()
                .HasForeignKey(a => a.PointInteretId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(a => a.Itineraire)
                .WithMany(i => i.Arrets)
                .HasForeignKey(a => a.ItineraireId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
        });
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<Voie> Voies { get; set; }
    public DbSet<Signal> Signaux { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<PointInteret> PointsInteret { get; set; }
    public DbSet<Itineraire> Itineraires { get; set; }
    public DbSet<ItineraireArret> ItineraireArrets { get; set; }
    public DbSet<Inspection> Inspections { get; set; }


    public void SeedData()
    {
        if (Users.Any() || Stations.Any() || Trains.Any())
            return;

        var gareQuebec = new Station
        {
            Nom = "Gare Québec-Gatineau",
            Longitude = -71.33444099998634,
            Latitude = 46.79562789552937,
            CapaciteMaximale = 50
        };

        var garePalais = new Station
        {
            Nom = "Gare du Palais",
            Longitude = -71.21337677932661,
            Latitude = 46.820120150583605,
            CapaciteMaximale = 40
        };

        var gareCN = new Station
        {
            Nom = "Gare CN",
            Longitude = -71.30054954776286,
            Latitude = 46.753797998277804,
            CapaciteMaximale = 30
        };

        Stations.AddRange(gareQuebec, garePalais, gareCN);
        SaveChanges();

        var voies = new List<Voie>
        {
            new Voie { Numero = "V1", EstDisponible = true, StationId = gareQuebec.Id },
            new Voie { Numero = "V2", EstDisponible = true, StationId = gareQuebec.Id },
            new Voie { Numero = "V1", EstDisponible = true, StationId = garePalais.Id },
            new Voie { Numero = "V2", EstDisponible = true, StationId = garePalais.Id },
            new Voie { Numero = "V1", EstDisponible = true, StationId = gareCN.Id },
            new Voie { Numero = "V2", EstDisponible = true, StationId = gareCN.Id }
        };
        Voies.AddRange(voies);
        SaveChanges();

        var blockQC_Gatineau = new Block
        {
            Nom = "Block Québec-Gatineau",
            EstOccupe = false,
            LatitudeDebut = gareQuebec.Latitude,
            LongitudeDebut = gareQuebec.Longitude,
            LatitudeFin = 46.770566303754286,
            LongitudeFin = -71.42840739803563
        };


        var blockQC_Station_Quebec_Intersection_Droite = new Block
        {
            Nom = "Block Station Québec-Intersection-Droite",
            EstOccupe = false,
            LatitudeDebut = gareQuebec.Latitude,
            LongitudeDebut = gareQuebec.Longitude,
            LatitudeFin = 46.79963795762025,
            LongitudeFin = -71.29427988745243
        };

        var blockQC_Station_Quebec_Intersection_Droite_Vers_Intersection_Bas = new Block
        {
            Nom = "Block Station Québec-Intersection-Droite-Vers-Intersection-Bas",
            EstOccupe = false,
            LatitudeDebut = 46.79833264776405,
            LongitudeDebut = -71.28913182685626,
            LatitudeFin = 46.749359107809326,
            LongitudeFin = -71.33773800862603
        };

        var blockQC_Intersection_Bas_Vers_Point_Nord = new Block
        {
            Nom = "Block Interesction-Bas-Vers-Point-Vers-Nord",
            EstOccupe = false,
            LatitudeDebut = 46.7477196113329,
            LongitudeDebut = -71.34014168123579,
            LatitudeFin = 46.76402942764486,
            LongitudeFin = -71.42372253021989
        };

        var blockQC_Intersection_Bas_Vers_Station_CN = new Block
        {
            Nom = "Block Intersection-Bas-Vers-Station-CN",
            EstOccupe = false,
            LatitudeDebut = 46.747917956551255,
            LongitudeDebut = -71.33452884292359,
            LatitudeFin = gareCN.Latitude,
            LongitudeFin = gareCN.Longitude
        };

        var blockQC_Station_CN_Vers_Point_Rive_Sud = new Block
        {
            Nom = "Block Station CN-Vers-Point-Rive-sud",
            EstOccupe = false,
            LatitudeDebut = gareCN.Latitude,
            LongitudeDebut = gareCN.Longitude,
            LatitudeFin = 46.74217099294358,
            LongitudeFin = -71.28598704295527
        };

        var blockQC_Station_CN_Vers_Point_Distribution = new Block
        {
            Nom = "Block Station CN-Vers-Point-Distribution",
            EstOccupe = false,
            LatitudeDebut = gareCN.Latitude,
            LongitudeDebut = gareCN.Longitude,
            LatitudeFin = 46.79123413308056,
            LongitudeFin = -71.23086825750235
        };

        var blockQC_Point_Distribution_Vers_Intersection_Gauche = new Block
        {
            Nom = "Block Point-Distribution-Vers-Intersection-Gauche",
            EstOccupe = false,
            LatitudeDebut = 46.79097,
            LongitudeDebut = -71.23043,
            LatitudeFin = 46.80049888631939,
            LongitudeFin = -71.28665573603986
        };

        var blockQC_Station_Quebec_Intersection_Droite_Vers_Intersection_Haut = new Block
        {
            Nom = "Block Station Quebec-Intersection-Droite-Vers-Intersection-Haut",
            EstOccupe = false,
            LatitudeDebut = 46.80049888631939,
            LongitudeDebut = -71.28665573603986,
            LatitudeFin = 46.829843408893616,
            LongitudeFin = -71.2252855058278
        };

        var blockQC_Station_Palais_Intersection_Haut_Vers_Point_Charlevoix = new Block
        {
            Nom = "Block Palais-Intersection-Haut-Vers-Point-Charlevoix",
            EstOccupe = false,
            LatitudeDebut = 46.829843408893616,
            LongitudeDebut = -71.225285505827800,
            LatitudeFin = 46.846606091346985,
            LongitudeFin = -71.206813389556120,
        };

        var blockQC_Station_Palais_Intersection_Haut_Vers_Point_Baie = new Block
        {
            Nom = "Block Palais-Intersection-Haut-Vers-Point-Baie",
            EstOccupe = false,
            LatitudeDebut = 46.82786754367638,
            LongitudeDebut = -71.21633508142588,
            LatitudeFin = 46.83748643691269,
            LongitudeFin = -71.20723922102754
        };

        var blockQC_Station_Palais_Intersection_Haut_Vers_Station_Palais = new Block
        {
            Nom = "Block Palais-Intersection-Haut-Vers-Station-Palais",
            EstOccupe = false,
            LatitudeDebut = 46.82291921145765,
            LongitudeDebut = -71.21733178754408,
            LatitudeFin = garePalais.Latitude,
            LongitudeFin = garePalais.Longitude
        };

        var blockQC_Station_Palais_Vers_Point_Port = new Block
        {
            Nom = "Block Station Palais-Vers-Point-Port",
            EstOccupe = false,
            LatitudeDebut = garePalais.Latitude,
            LongitudeDebut = garePalais.Longitude,
            LatitudeFin = 46.823257873570675,
            LongitudeFin = -71.19743280571853
        };


        Blocks.AddRange(blockQC_Gatineau, blockQC_Station_Quebec_Intersection_Droite, blockQC_Station_Quebec_Intersection_Droite_Vers_Intersection_Bas, blockQC_Intersection_Bas_Vers_Point_Nord, blockQC_Intersection_Bas_Vers_Station_CN, blockQC_Station_CN_Vers_Point_Rive_Sud, blockQC_Station_CN_Vers_Point_Distribution, blockQC_Point_Distribution_Vers_Intersection_Gauche, blockQC_Station_Quebec_Intersection_Droite_Vers_Intersection_Haut,
                       blockQC_Station_Palais_Intersection_Haut_Vers_Point_Charlevoix, blockQC_Station_Palais_Intersection_Haut_Vers_Point_Baie, blockQC_Station_Palais_Intersection_Haut_Vers_Station_Palais, blockQC_Station_Palais_Vers_Point_Port);
        SaveChanges();

        var train1 = new Train
        {
            Numero = "T-QC-101",
            Type = TypeTrain.Passagers,
            Etat = EtatTrain.EnGare,
            Capacite = 200,
            StationActuelleId = gareQuebec.Id,
            VoieActuelleId = voies[0].Id,
            BlockActuelId = null
        };

        var train2 = new Train
        {
            Numero = "T-QC-202",
            Type = TypeTrain.Passagers,
            Etat = EtatTrain.EnGare,
            Capacite = 150,
            StationActuelleId = garePalais.Id,
            VoieActuelleId = voies[2].Id,
            BlockActuelId = null
        };

        Trains.AddRange(train1, train2);
        SaveChanges();

        var users = new List<User>
        {
            new User { Prenom = "admin", Nom = "admin", Username = "admin", Password = "admin123", Role = Role.Administrateur, StationAssigneeId = gareQuebec.Id },
            new User { Prenom = "Jean", Nom = "Tremblay", Username = "jtremblay", Password = "password123", Role = Role.Employe, StationAssigneeId = gareQuebec.Id },
            new User { Prenom = "Marie", Nom = "Gagnon", Username = "mgagnon", Password = "password123", Role = Role.Employe, StationAssigneeId = garePalais.Id },
            new User { Prenom = "Paul", Nom = "Kirk", Username = "plkirk", Password = "password123", Role = Role.Mecanicien, StationAssigneeId = garePalais.Id }
        };

        Users.AddRange(users);
        SaveChanges();

        var pointsInteret = new List<PointInteret>
        {
            new PointInteret { Nom = "Vers Charlevoix", Type = "Destination", Longitude = -71.20681338955612, Latitude = 46.846606091346985, Description = "Direction vers Charlevoix" },
            new PointInteret { Nom = "Baie de Beauport", Type = "Plage", Longitude = -71.20723922102754, Latitude = 46.83748643691269, Description = "Plage de Beauport" },
            new PointInteret { Nom = "Port de Québec", Type = "Port", Longitude = -71.19743280571853, Latitude = 46.823257873570675, Description = "Port de Québec" },
            new PointInteret { Nom = "Centre de distribution", Type = "Logistique", Longitude = -71.23086825750235, Latitude = 46.79123413308056, Description = "Centre de distribution" },
            new PointInteret { Nom = "Vers la rive-sud", Type = "Destination", Longitude = -71.28598704295527, Latitude = 46.74217099294358, Description = "Direction Rive-Sud" },
            new PointInteret { Nom = "Vers Gatineau", Type = "Destination", Longitude = -71.42840739803563, Latitude = 46.770566303754286, Description = "Direction Gatineau" },
            new PointInteret { Nom = "Vers le nord", Type = "Destination", Longitude = -71.42372253021989, Latitude = 46.76402942764486, Description = "Direction Nord" }
        };

        PointsInteret.AddRange(pointsInteret);
        SaveChanges();
    }


}
