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
        public DbSet<ChannelReading> ChannelReadings { get; set; }


        public FacilityContext() {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public FacilityContext(DbContextOptions<FacilityContext> options) : base(options) {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=FacilityMonitoringTesting;" +
                "User Id=***;Password=***;");
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<MonitoringBox>().HasBaseType<ModbusDevice>();
            builder.Entity<DiscreteInput>().HasBaseType<Channel>();
            builder.Entity<AnalogInput>().HasBaseType<Channel>();
            builder.Entity<DiscreteAlert>().HasBaseType<Alert>();
            builder.Entity<AnalogAlert>().HasBaseType<Alert>();
            builder.Entity<DiscreteReading>().HasBaseType<ChannelReading>();
            builder.Entity<AnalogReading>().HasBaseType<ChannelReading>();

            builder.Entity<Channel>()
                .OwnsOne(p => p.ChannelAddress);

            builder.Entity<Channel>()
                .OwnsOne(p => p.ModbusAddress);

            builder.Entity<ModbusDevice>()
                .OwnsOne(p => p.NetworkConfiguration);

            builder.Entity<FacilityAction>()
                .OwnsMany(p => p.ActionOutputs, a => {
                    a.WithOwner().HasForeignKey("OwnerId");
                    a.Property<int>("Id");
                    a.HasKey("Id");
                });

        }

    }
}
