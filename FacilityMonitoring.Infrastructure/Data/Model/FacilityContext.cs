using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace FacilityMonitoring.Infrastructure.Data.Model {
    public class FacilityContext:DbContext {
        public DbSet<ModbusDevice> ModbusDevices { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<FacilityAction> FacilityActions { get; set; }
        public DbSet<FacilityZone> FacilityZones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=FacilityMonitoringTesting;" +
                "User Id=aelmendorf;Password=Drizzle123!;");
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            //Modbus Device inheritance
            builder.Entity<MonitoringBox>().HasBaseType<ModbusDevice>();
            //Channel inheritance
            builder.Entity<DiscreteInput>().HasBaseType<Channel>();
            builder.Entity<AnalogInput>().HasBaseType<Channel>();
            //Alert inheritance
            builder.Entity<DiscreteAlert>().HasBaseType<Alert>();
            builder.Entity<AnalogAlert>().HasBaseType<Alert>();

            builder.Entity<Channel>()
                .OwnsOne(p => p.ChannelAddress);

            builder.Entity<Channel>()
                .OwnsOne(p => p.ModbusAddress);

            builder.Entity<ModbusDevice>()
                .OwnsOne(p => p.NetworkConfiguration)
                .OwnsOne(p => p.ModbusConfig);

            builder.Entity<FacilityAction>()
                .OwnsMany(p => p.ActionOutputs, a => {
                    a.WithOwner().HasForeignKey("OwnerId");
                    a.Property<int>("Id");
                    a.HasKey("Id");
                });

            builder.Entity<FacilityZone>()
                .OwnsOne(p => p.ZoneSize);

            builder.Entity<FacilityZone>()
                .OwnsOne(p => p.Location);

            builder.Entity<ModbusDevice>()
                .HasMany(p => p.Channels)
                .WithOne(p => p.ModbusDevice)
                .HasForeignKey(p => p.ModbusDeviceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MonitoringBox>()
                .HasMany(p => p.Modules)
                .WithMany(p => p.MonitoringBoxes)
                .UsingEntity(j => j.ToTable("BoxModules"));

            builder.Entity<Alert>()
                .HasOne(p => p.FacilityAction)
                .WithMany(p => p.Alerts)
                .HasForeignKey(p => p.FacilityActionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<DiscreteInput>()
                .HasOne(p => p.DiscreteAlert)
                .WithOne(p => p.Channel as DiscreteInput)
                .HasForeignKey<DiscreteAlert>(e => e.ChannelId)
                .IsRequired(false);

            builder.Entity<AnalogInput>()
                .HasMany(p => p.AnalogAlerts)
                .WithOne(p => p.Channel as AnalogInput)
                .HasForeignKey(e => e.ChannelId)
                .IsRequired(false); 

            builder.Entity<AnalogInput>()
                .HasOne(p => p.Sensor)
                .WithMany(p => p.AnalogInputs)
                .HasForeignKey(p => p.SensorId)
                .IsRequired(false);

            builder.Entity<FacilityAction>()
                .HasMany(p => p.Alerts)
                .WithOne(p => p.FacilityAction)
                .HasForeignKey(p => p.FacilityActionId)
                .IsRequired(true);

            builder.Entity<ModbusDevice>()
                .HasMany(p => p.Zones)
                .WithMany(p => p.ModbusDevices)
                .UsingEntity(j => j.ToTable("DeviceZones"));

            builder.Entity<Channel>()
                .HasMany(p => p.Zones)
                .WithMany(p => p.Channels)
                .UsingEntity(j => j.ToTable("ChannelZones"));

            builder.Entity<FacilityZone>()
                .HasOne(p => p.ZoneParent)
                .WithMany(p => p.SubZones)
                .HasForeignKey(p => p.ZoneParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }

    public class FacilityContextFactory : IDesignTimeDbContextFactory<FacilityContext> {
        public FacilityContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<FacilityContext>();
            return new FacilityContext();
        }
    }
}
