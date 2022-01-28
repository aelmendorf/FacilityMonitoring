﻿// <auto-generated />
using System;
using FacilityMonitoring.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    [DbContext(typeof(FacilityContext))]
    [Migration("20220128143333_moveSlaveAddr")]
    partial class moveSlaveAddr
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ChannelFacilityZone", b =>
                {
                    b.Property<int>("ChannelsId")
                        .HasColumnType("int");

                    b.Property<int>("ZonesId")
                        .HasColumnType("int");

                    b.HasKey("ChannelsId", "ZonesId");

                    b.HasIndex("ZonesId");

                    b.ToTable("ChannelZones", (string)null);
                });

            modelBuilder.Entity("DeviceFacilityZone", b =>
                {
                    b.Property<int>("DevicesId")
                        .HasColumnType("int");

                    b.Property<int>("ZonesId")
                        .HasColumnType("int");

                    b.HasKey("DevicesId", "ZonesId");

                    b.HasIndex("ZonesId");

                    b.ToTable("DeviceZones", (string)null);
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Alert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Bypass")
                        .HasColumnType("bit");

                    b.Property<int>("BypassResetTime")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<int?>("FacilityActionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FacilityActionId");

                    b.ToTable("Alerts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Alert");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Bypass")
                        .HasColumnType("bit");

                    b.Property<bool>("Connected")
                        .HasColumnType("bit");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Display")
                        .HasColumnType("bit");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ModbusDeviceId")
                        .HasColumnType("int");

                    b.Property<int>("SystemChannel")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ModbusDeviceId");

                    b.ToTable("Channels");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Channel");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("BypassAlarms")
                        .HasColumnType("bit");

                    b.Property<string>("DataReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ReadInterval")
                        .HasColumnType("float");

                    b.Property<double>("SaveInterval")
                        .HasColumnType("float");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Devices");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Device");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.FacilityAction", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ActionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ActionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("FacilityActions");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.FacilityZone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<string>("ZoneName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ZoneParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ZoneParentId");

                    b.ToTable("FacilityZones");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ChannelCount")
                        .HasColumnType("int");

                    b.Property<int>("ModuleChannel")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Slot")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Modules");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ChannelCount = 16,
                            ModuleChannel = 1,
                            Name = "P1-16ND3",
                            Slot = 1
                        },
                        new
                        {
                            Id = 2,
                            ChannelCount = 16,
                            ModuleChannel = 1,
                            Name = "P1-16ND3",
                            Slot = 2
                        },
                        new
                        {
                            Id = 3,
                            ChannelCount = 8,
                            ModuleChannel = 0,
                            Name = "P1-08ADL-1",
                            Slot = 3
                        },
                        new
                        {
                            Id = 4,
                            ChannelCount = 8,
                            ModuleChannel = 0,
                            Name = "P1-08ADL-1",
                            Slot = 4
                        },
                        new
                        {
                            Id = 5,
                            ChannelCount = 8,
                            ModuleChannel = 2,
                            Name = "P1-08TD2",
                            Slot = 5
                        });
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Factor")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Offset")
                        .HasColumnType("float");

                    b.Property<double>("Slope")
                        .HasColumnType("float");

                    b.Property<string>("Units")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sensors");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "H2 Gas Detector",
                            Factor = 1.0,
                            Name = "H2 Detector-PPM",
                            Offset = -250.0,
                            Slope = 62.5,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 2,
                            Description = "O2 Gas Detector",
                            DisplayName = "O2",
                            Factor = 1.0,
                            Name = "O2 Detector",
                            Offset = -18.75,
                            Slope = 4.6900000000000004,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 3,
                            Description = "NH3 Gas Detector",
                            DisplayName = "NH3",
                            Factor = 1.0,
                            Name = "NH3 Detector",
                            Offset = -6.25,
                            Slope = 1.5600000000000001,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 4,
                            Description = "N2 Gas Detector",
                            DisplayName = "N2",
                            Factor = 1.0,
                            Name = "N2 Detector",
                            Offset = -140.0,
                            Slope = 5.0,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 5,
                            Description = "H2 Explosion Gas Detector",
                            DisplayName = "H2-LEL",
                            Factor = 1.0,
                            Name = "H2 LEL Detector",
                            Offset = -25.0,
                            Slope = 6.25,
                            Units = "LEL"
                        });
                });

            modelBuilder.Entity("ModuleMonitoringBox", b =>
                {
                    b.Property<int>("ModulesId")
                        .HasColumnType("int");

                    b.Property<int>("MonitoringBoxesId")
                        .HasColumnType("int");

                    b.HasKey("ModulesId", "MonitoringBoxesId");

                    b.HasIndex("MonitoringBoxesId");

                    b.ToTable("BoxModules", (string)null);
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.AnalogAlert", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Alert");

                    b.Property<int?>("AnalogInputId")
                        .HasColumnType("int");

                    b.Property<double>("SetPoint")
                        .HasColumnType("float");

                    b.HasIndex("AnalogInputId");

                    b.HasDiscriminator().HasValue("AnalogAlert");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.AnalogInput", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Channel");

                    b.Property<int?>("SensorId")
                        .HasColumnType("int");

                    b.HasIndex("SensorId");

                    b.HasDiscriminator().HasValue("AnalogInput");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.ApiDevice", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Device");

                    b.Property<string>("ApiToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ApiDevice");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.BnetDevice", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Device");

                    b.HasDiscriminator().HasValue("BnetDevice");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.DiscreteAlert", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Alert");

                    b.Property<int?>("DiscreteInputId")
                        .HasColumnType("int");

                    b.Property<int>("TriggerOn")
                        .HasColumnType("int");

                    b.HasIndex("DiscreteInputId")
                        .IsUnique()
                        .HasFilter("[DiscreteInputId] IS NOT NULL");

                    b.HasDiscriminator().HasValue("DiscreteAlert");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.DiscreteInput", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Channel");

                    b.HasDiscriminator().HasValue("DiscreteInput");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.DiscreteOutput", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Channel");

                    b.Property<int>("ChannelState")
                        .HasColumnType("int");

                    b.Property<int>("StartState")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("DiscreteOutput");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.ModbusDevice", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Device");

                    b.HasDiscriminator().HasValue("ModbusDevice");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.VirtualInput", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.Channel");

                    b.Property<int?>("VirtualAlertId")
                        .HasColumnType("int");

                    b.HasIndex("VirtualAlertId");

                    b.HasDiscriminator().HasValue("VirtualInput");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.MonitoringBox", b =>
                {
                    b.HasBaseType("FacilityMonitoring.Infrastructure.Data.Model.ModbusDevice");

                    b.HasDiscriminator().HasValue("MonitoringBox");
                });

            modelBuilder.Entity("ChannelFacilityZone", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.Channel", null)
                        .WithMany()
                        .HasForeignKey("ChannelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.FacilityZone", null)
                        .WithMany()
                        .HasForeignKey("ZonesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DeviceFacilityZone", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.Device", null)
                        .WithMany()
                        .HasForeignKey("DevicesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.FacilityZone", null)
                        .WithMany()
                        .HasForeignKey("ZonesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Alert", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.FacilityAction", "FacilityAction")
                        .WithMany("Alerts")
                        .HasForeignKey("FacilityActionId");

                    b.Navigation("FacilityAction");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Channel", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusDevice", "ModbusDevice")
                        .WithMany("Channels")
                        .HasForeignKey("ModbusDeviceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ChannelAddress", "ChannelAddress", b1 =>
                        {
                            b1.Property<int>("ChannelId")
                                .HasColumnType("int");

                            b1.Property<int>("Channel")
                                .HasColumnType("int");

                            b1.Property<int>("ModuleSlot")
                                .HasColumnType("int");

                            b1.HasKey("ChannelId");

                            b1.ToTable("Channels");

                            b1.WithOwner()
                                .HasForeignKey("ChannelId");
                        });

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusAddress", "ModbusAddress", b1 =>
                        {
                            b1.Property<int>("ChannelId")
                                .HasColumnType("int");

                            b1.Property<int>("Address")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterLength")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterType")
                                .HasColumnType("int");

                            b1.HasKey("ChannelId");

                            b1.ToTable("Channels");

                            b1.WithOwner()
                                .HasForeignKey("ChannelId");
                        });

                    b.Navigation("ChannelAddress");

                    b.Navigation("ModbusAddress");

                    b.Navigation("ModbusDevice");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.FacilityAction", b =>
                {
                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusAddress", "ModbusAddress", b1 =>
                        {
                            b1.Property<int>("FacilityActionId")
                                .HasColumnType("int");

                            b1.Property<int>("Address")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterLength")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterType")
                                .HasColumnType("int");

                            b1.HasKey("FacilityActionId");

                            b1.ToTable("FacilityActions");

                            b1.WithOwner()
                                .HasForeignKey("FacilityActionId");
                        });

                    b.OwnsMany("FacilityMonitoring.Infrastructure.Data.Model.ActionOutput", "ActionOutputs", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<int>("OffLevel")
                                .HasColumnType("int");

                            b1.Property<int>("OnLevel")
                                .HasColumnType("int");

                            b1.Property<int?>("OutputId")
                                .HasColumnType("int");

                            b1.Property<int>("OwnerId")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("OutputId");

                            b1.HasIndex("OwnerId");

                            b1.ToTable("ActionOutput");

                            b1.HasOne("FacilityMonitoring.Infrastructure.Data.Model.DiscreteOutput", "Output")
                                .WithMany()
                                .HasForeignKey("OutputId");

                            b1.WithOwner()
                                .HasForeignKey("OwnerId");

                            b1.Navigation("Output");
                        });

                    b.Navigation("ActionOutputs");

                    b.Navigation("ModbusAddress");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.FacilityZone", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.FacilityZone", "ZoneParent")
                        .WithMany("SubZones")
                        .HasForeignKey("ZoneParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.Location", "Location", b1 =>
                        {
                            b1.Property<int>("FacilityZoneId")
                                .HasColumnType("int");

                            b1.Property<double>("XCoord")
                                .HasColumnType("float");

                            b1.Property<double>("YCoord")
                                .HasColumnType("float");

                            b1.HasKey("FacilityZoneId");

                            b1.ToTable("FacilityZones");

                            b1.WithOwner()
                                .HasForeignKey("FacilityZoneId");
                        });

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ZoneSize", "ZoneSize", b1 =>
                        {
                            b1.Property<int>("FacilityZoneId")
                                .HasColumnType("int");

                            b1.Property<double>("Height")
                                .HasColumnType("float");

                            b1.Property<double>("Width")
                                .HasColumnType("float");

                            b1.HasKey("FacilityZoneId");

                            b1.ToTable("FacilityZones");

                            b1.WithOwner()
                                .HasForeignKey("FacilityZoneId");
                        });

                    b.Navigation("Location");

                    b.Navigation("ZoneParent");

                    b.Navigation("ZoneSize");
                });

            modelBuilder.Entity("ModuleMonitoringBox", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.Module", null)
                        .WithMany()
                        .HasForeignKey("ModulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.MonitoringBox", null)
                        .WithMany()
                        .HasForeignKey("MonitoringBoxesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.AnalogAlert", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.AnalogInput", "AnalogInput")
                        .WithMany("AnalogAlerts")
                        .HasForeignKey("AnalogInputId");

                    b.Navigation("AnalogInput");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.AnalogInput", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.Sensor", "Sensor")
                        .WithMany("AnalogInputs")
                        .HasForeignKey("SensorId");

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusAddress", "AlertAddress", b1 =>
                        {
                            b1.Property<int>("AnalogInputId")
                                .HasColumnType("int");

                            b1.Property<int>("Address")
                                .ValueGeneratedOnUpdateSometimes()
                                .HasColumnType("int");

                            b1.Property<int>("RegisterLength")
                                .ValueGeneratedOnUpdateSometimes()
                                .HasColumnType("int");

                            b1.Property<int>("RegisterType")
                                .ValueGeneratedOnUpdateSometimes()
                                .HasColumnType("int");

                            b1.HasKey("AnalogInputId");

                            b1.ToTable("Channels");

                            b1.WithOwner()
                                .HasForeignKey("AnalogInputId");
                        });

                    b.Navigation("AlertAddress");

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.DiscreteAlert", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.DiscreteInput", "DiscreteInput")
                        .WithOne("DiscreteAlert")
                        .HasForeignKey("FacilityMonitoring.Infrastructure.Data.Model.DiscreteAlert", "DiscreteInputId");

                    b.Navigation("DiscreteInput");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.DiscreteInput", b =>
                {
                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusAddress", "AlertAddress", b1 =>
                        {
                            b1.Property<int>("DiscreteInputId")
                                .HasColumnType("int");

                            b1.Property<int>("Address")
                                .ValueGeneratedOnUpdateSometimes()
                                .HasColumnType("int");

                            b1.Property<int>("RegisterLength")
                                .ValueGeneratedOnUpdateSometimes()
                                .HasColumnType("int");

                            b1.Property<int>("RegisterType")
                                .ValueGeneratedOnUpdateSometimes()
                                .HasColumnType("int");

                            b1.HasKey("DiscreteInputId");

                            b1.ToTable("Channels");

                            b1.WithOwner()
                                .HasForeignKey("DiscreteInputId");
                        });

                    b.Navigation("AlertAddress");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.ModbusDevice", b =>
                {
                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ChannelRegisterMapping", "ChannelMapping", b1 =>
                        {
                            b1.Property<int>("ModbusDeviceId")
                                .HasColumnType("int");

                            b1.Property<int>("ActionRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("ActionStart")
                                .HasColumnType("int");

                            b1.Property<int>("ActionStop")
                                .HasColumnType("int");

                            b1.Property<int>("AlertRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("AlertStart")
                                .HasColumnType("int");

                            b1.Property<int>("AlertStop")
                                .HasColumnType("int");

                            b1.Property<int>("AnalogRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("AnalogStart")
                                .HasColumnType("int");

                            b1.Property<int>("AnalogStop")
                                .HasColumnType("int");

                            b1.Property<int>("DeviceRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("DeviceStart")
                                .HasColumnType("int");

                            b1.Property<int>("DeviceStop")
                                .HasColumnType("int");

                            b1.Property<int>("DiscreteRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("DiscreteStart")
                                .HasColumnType("int");

                            b1.Property<int>("DiscreteStop")
                                .HasColumnType("int");

                            b1.Property<int>("OutputRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("OutputStart")
                                .HasColumnType("int");

                            b1.Property<int>("OutputStop")
                                .HasColumnType("int");

                            b1.Property<int>("VirtualRegisterType")
                                .HasColumnType("int");

                            b1.Property<int>("VirtualStart")
                                .HasColumnType("int");

                            b1.Property<int>("VirtualStop")
                                .HasColumnType("int");

                            b1.HasKey("ModbusDeviceId");

                            b1.ToTable("Devices");

                            b1.WithOwner()
                                .HasForeignKey("ModbusDeviceId");
                        });

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusAddress", "ModbusAddress", b1 =>
                        {
                            b1.Property<int>("ModbusDeviceId")
                                .HasColumnType("int");

                            b1.Property<int>("Address")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterLength")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterType")
                                .HasColumnType("int");

                            b1.HasKey("ModbusDeviceId");

                            b1.ToTable("Devices");

                            b1.WithOwner()
                                .HasForeignKey("ModbusDeviceId");
                        });

                    b.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.NetworkConfiguration", "NetworkConfiguration", b1 =>
                        {
                            b1.Property<int>("ModbusDeviceId")
                                .HasColumnType("int");

                            b1.Property<string>("DNS")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Gateway")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("IPAddress")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("MAC")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Port")
                                .HasColumnType("int");

                            b1.HasKey("ModbusDeviceId");

                            b1.ToTable("Devices");

                            b1.WithOwner()
                                .HasForeignKey("ModbusDeviceId");

                            b1.OwnsOne("FacilityMonitoring.Infrastructure.Data.Model.ModbusConfig", "ModbusConfig", b2 =>
                                {
                                    b2.Property<int>("NetworkConfigurationModbusDeviceId")
                                        .HasColumnType("int");

                                    b2.Property<int>("Coils")
                                        .HasColumnType("int");

                                    b2.Property<int>("DiscreteInputs")
                                        .HasColumnType("int");

                                    b2.Property<int>("HoldingRegisters")
                                        .HasColumnType("int");

                                    b2.Property<int>("InputRegisters")
                                        .HasColumnType("int");

                                    b2.Property<int>("SlaveAddress")
                                        .HasColumnType("int");

                                    b2.HasKey("NetworkConfigurationModbusDeviceId");

                                    b2.ToTable("Devices");

                                    b2.WithOwner()
                                        .HasForeignKey("NetworkConfigurationModbusDeviceId");
                                });

                            b1.Navigation("ModbusConfig");
                        });

                    b.Navigation("ChannelMapping");

                    b.Navigation("ModbusAddress");

                    b.Navigation("NetworkConfiguration");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.VirtualInput", b =>
                {
                    b.HasOne("FacilityMonitoring.Infrastructure.Data.Model.DiscreteAlert", "VirtualAlert")
                        .WithMany()
                        .HasForeignKey("VirtualAlertId");

                    b.Navigation("VirtualAlert");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.FacilityAction", b =>
                {
                    b.Navigation("Alerts");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.FacilityZone", b =>
                {
                    b.Navigation("SubZones");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.Sensor", b =>
                {
                    b.Navigation("AnalogInputs");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.AnalogInput", b =>
                {
                    b.Navigation("AnalogAlerts");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.DiscreteInput", b =>
                {
                    b.Navigation("DiscreteAlert");
                });

            modelBuilder.Entity("FacilityMonitoring.Infrastructure.Data.Model.ModbusDevice", b =>
                {
                    b.Navigation("Channels");
                });
#pragma warning restore 612, 618
        }
    }
}
