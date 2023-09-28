using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;

namespace LcsServer.DatabaseLayer
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<User> Users => Set<User>();
        public DbSet<SensorValue> SensorValues => Set<SensorValue>();
        public DbSet<SensorUnit> SensorUnits => Set<SensorUnit>();
        public DbSet<SensorDb> Sensors => Set<SensorDb>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<DeviceParam> DeviceParams => Set<DeviceParam>();
        public DbSet<Command> Commands => Set<Command>();
        public DbSet<Settings> Settings => Set<Settings>();
        public DbSet<StatusOfDevice> DeviceStatuses => Set<StatusOfDevice>();
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        public DatabaseContext()
        {
        }

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             var pathToConfig = Path.GetDirectoryName(Environment.ProcessPath);
             var configuration = new ConfigurationBuilder()
                 .SetBasePath(pathToConfig)
                 .AddJsonFile("appsettings.json")
                 .Build();

             var connectionString = configuration.GetConnectionString("DefaultConnection");
             optionsBuilder.UseSqlite(connectionString);
         }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.level).HasMaxLength(250);
                entity.Property(e => e.dateTime).HasColumnType("NUMERIC");
                entity.Property(e => e.deviceId).HasDefaultValue("");
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.State).HasDefaultValue("Unread");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Login).HasMaxLength(250);
                entity.Property(e => e.Password).HasMaxLength(500);
            });

            modelBuilder.Entity<SensorDb>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.deviceId).HasMaxLength(500);
            });

            modelBuilder.Entity<SensorValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SensorId).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Timestamp).HasColumnType("NUMERIC");
                entity.Property(e => e.Value).HasColumnType("REAL");
                entity.Property(e => e.NormalMinValue).HasColumnType("INTEGER");
                entity.Property(e => e.NormalMaxValue).HasColumnType("INTEGER");
                entity.Property(e => e.SensorUnitId).HasColumnType("INTEGER");

            });

            modelBuilder.Entity<SensorUnit>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e=>e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<User>().HasData(
                    new User() { Id = 1, Login = "admin", Password = "admin", Role = "admin"},
                    new User() { Id = 2, Login = "user", Password = "user", Role = "user"}
                );

            modelBuilder.Entity<SensorUnit>().HasData(
                    new SensorUnit() { DbId = 1, Id = 0, Name= "None" },
                    new SensorUnit() { DbId = 2, Id = 1, Name = "Centigrade" },
                    new SensorUnit() { DbId = 3, Id = 2, Name = "VoltsDC" },
                    new SensorUnit() { DbId = 4, Id = 3, Name = "VoltsACPeak" },
                    new SensorUnit() { DbId = 5, Id = 4, Name = "VoltsACRms" },
                    new SensorUnit() { DbId = 6, Id = 5, Name = "AmpereDC" },
                    new SensorUnit() { DbId = 7,  Id = 6, Name = "AmpereACPeak" },
                    new SensorUnit() { DbId = 8, Id = 7, Name = "AmpereACRms" },
                    new SensorUnit() { DbId = 9, Id = 8, Name = "Hertz" },
                    new SensorUnit() { DbId = 10, Id = 9, Name = "Ohm" },
                    new SensorUnit() { DbId = 11, Id = 10, Name = "Watt" },
                    new SensorUnit() { DbId = 12, Id = 11, Name = "Kilogram" },
                    new SensorUnit() { DbId = 13, Id = 12, Name = "Meters" },
                    new SensorUnit() { DbId = 14, Id = 13, Name = "MetersSquared" },
                    new SensorUnit() { DbId = 15, Id = 14, Name = "MetersCubed" },
                    new SensorUnit() { DbId = 16, Id = 15, Name="KilogramsPerMeterCubed"},
                    new SensorUnit() { DbId = 17, Id = 16, Name = "MetersPerSecond" },
                    new SensorUnit() { DbId = 18, Id = 17, Name = "MetersPerSecondSquared" },
                    new SensorUnit() { DbId = 19, Id = 18, Name = "Newton" },
                    new SensorUnit() { DbId = 20, Id = 19, Name = "Joule" },
                    new SensorUnit() { DbId = 21, Id = 20, Name = "Pascal" },
                    new SensorUnit() { DbId = 22, Id = 21, Name = "Second" },
                    new SensorUnit() { DbId = 23, Id = 22, Name = "Degree" },
                    new SensorUnit() { DbId = 24, Id = 23, Name = "Steradian" },
                    new SensorUnit() { DbId = 25, Id = 24, Name = "Candela" },
                    new SensorUnit() { DbId = 26, Id = 25, Name = "Lumen" },
                    new SensorUnit() { DbId = 27, Id = 26, Name = "Lux" },
                    new SensorUnit() { DbId = 28, Id = 27, Name = "Ire" },
                    new SensorUnit() { DbId = 29, Id = 28, Name = "Byte" }
                );

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(250);
                entity.Property(e => e.IsEnabled);
            });

            modelBuilder.Entity<Settings>().HasData(
                new Settings() { Id=1,Name="RdmDiscoveryForbidden", IsEnabled = false},
                new Settings() { Id = 2, Name = "ModbusEnabled", IsEnabled = false },
                new Settings(){Id=3,Name="DeviceScanning", IsEnabled = false}
                );
            modelBuilder.Entity<StatusOfDevice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasColumnType("INTEGER");
            });
            
            modelBuilder.Entity<StatusOfDevice>().HasData(
                new StatusOfDevice() {Id = 1, Status = 0},
                new StatusOfDevice() {Id = 2, Status = 1},
                new StatusOfDevice() {Id = 3, Status = 2},
                new StatusOfDevice() {Id = 4, Status = 3},
                new StatusOfDevice() {Id = 5, Status = 4}
            );
            
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.deviceId).HasMaxLength(500);
            });

            modelBuilder.Entity<DeviceParam>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).HasMaxLength(500);
                entity.Property(e => e.ParamId).HasDefaultValue("");
                entity.Property(e => e.ParamName).HasMaxLength(500);
                entity.Property(e => e.ParamValue).HasMaxLength(500);
                entity.Property(e=>e.LastPoll).HasColumnType("NUMERIC");
            });

            modelBuilder.Entity<Command>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).HasMaxLength(500);
                entity.Property(e=>e.ParamId).HasMaxLength(150);
                entity.Property(e => e.ParamNewValue).HasMaxLength(150);
                entity.Property(e => e.State).HasColumnType("INTEGER");
            });
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext> 
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            var pathToConfig = Path.GetDirectoryName(Environment.ProcessPath);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(pathToConfig)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
