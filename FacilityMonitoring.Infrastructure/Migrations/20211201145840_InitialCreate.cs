using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacilityActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location_XCoord = table.Column<double>(type: "float", nullable: true),
                    Location_YCoord = table.Column<double>(type: "float", nullable: true),
                    ZoneSize_Width = table.Column<double>(type: "float", nullable: true),
                    ZoneSize_Height = table.Column<double>(type: "float", nullable: true),
                    Color = table.Column<int>(type: "int", nullable: false),
                    ZoneParentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityZones_FacilityZones_ZoneParentId",
                        column: x => x.ZoneParentId,
                        principalTable: "FacilityZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModbusDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkConfiguration_IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkConfiguration_DNS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkConfiguration_MAC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkConfiguration_Gateway = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetworkConfiguration_Port = table.Column<int>(type: "int", nullable: true),
                    NetworkConfiguration_SlaveAddress = table.Column<int>(type: "int", nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SlaveAddress = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BypassAlarms = table.Column<bool>(type: "bit", nullable: false),
                    ReadInterval = table.Column<double>(type: "float", nullable: false),
                    SaveInterval = table.Column<double>(type: "float", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    ChannelCount = table.Column<int>(type: "int", nullable: false),
                    ModuleChannel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZeroValue = table.Column<double>(type: "float", nullable: false),
                    Slope = table.Column<double>(type: "float", nullable: false),
                    Factor = table.Column<double>(type: "float", nullable: false),
                    Units = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceZones",
                columns: table => new
                {
                    ModbusDevicesId = table.Column<int>(type: "int", nullable: false),
                    ZonesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceZones", x => new { x.ModbusDevicesId, x.ZonesId });
                    table.ForeignKey(
                        name: "FK_DeviceZones_FacilityZones_ZonesId",
                        column: x => x.ZonesId,
                        principalTable: "FacilityZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceZones_ModbusDevices_ModbusDevicesId",
                        column: x => x.ModbusDevicesId,
                        principalTable: "ModbusDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoxModules",
                columns: table => new
                {
                    ModulesId = table.Column<int>(type: "int", nullable: false),
                    MonitoringBoxesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxModules", x => new { x.ModulesId, x.MonitoringBoxesId });
                    table.ForeignKey(
                        name: "FK_BoxModules_ModbusDevices_MonitoringBoxesId",
                        column: x => x.MonitoringBoxesId,
                        principalTable: "ModbusDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoxModules_Modules_ModulesId",
                        column: x => x.ModulesId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemChannel = table.Column<int>(type: "int", nullable: false),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    Connected = table.Column<bool>(type: "bit", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChannelAddress_Channel = table.Column<int>(type: "int", nullable: true),
                    ChannelAddress_ModbusSlot = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_Address = table.Column<int>(type: "int", nullable: true),
                    ModbusAddress_RegisterLength = table.Column<int>(type: "int", nullable: true),
                    ModbusDeviceId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: true),
                    ChannelState = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_ModbusDevices_ModbusDeviceId",
                        column: x => x.ModbusDeviceId,
                        principalTable: "ModbusDevices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Channels_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActionOutput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutputId = table.Column<int>(type: "int", nullable: true),
                    OnLevel = table.Column<int>(type: "int", nullable: false),
                    OffLevel = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionOutput", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionOutput_Channels_OutputId",
                        column: x => x.OutputId,
                        principalTable: "Channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActionOutput_FacilityActions_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "FacilityActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bypass = table.Column<bool>(type: "bit", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    FacilityActionId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetPoint = table.Column<double>(type: "float", nullable: true),
                    TriggerOn = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alerts_Channels_ChannelId1",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alerts_FacilityActions_FacilityActionId",
                        column: x => x.FacilityActionId,
                        principalTable: "FacilityActions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChannelZones",
                columns: table => new
                {
                    ChannelsId = table.Column<int>(type: "int", nullable: false),
                    ZonesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelZones", x => new { x.ChannelsId, x.ZonesId });
                    table.ForeignKey(
                        name: "FK_ChannelZones_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelZones_FacilityZones_ZonesId",
                        column: x => x.ZonesId,
                        principalTable: "FacilityZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionOutput_OutputId",
                table: "ActionOutput",
                column: "OutputId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionOutput_OwnerId",
                table: "ActionOutput",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ChannelId",
                table: "Alerts",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ChannelId1",
                table: "Alerts",
                column: "ChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FacilityActionId",
                table: "Alerts",
                column: "FacilityActionId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxModules_MonitoringBoxesId",
                table: "BoxModules",
                column: "MonitoringBoxesId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ModbusDeviceId",
                table: "Channels",
                column: "ModbusDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_SensorId",
                table: "Channels",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelZones_ZonesId",
                table: "ChannelZones",
                column: "ZonesId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceZones_ZonesId",
                table: "DeviceZones",
                column: "ZonesId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityZones_ZoneParentId",
                table: "FacilityZones",
                column: "ZoneParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionOutput");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "BoxModules");

            migrationBuilder.DropTable(
                name: "ChannelZones");

            migrationBuilder.DropTable(
                name: "DeviceZones");

            migrationBuilder.DropTable(
                name: "FacilityActions");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "FacilityZones");

            migrationBuilder.DropTable(
                name: "ModbusDevices");

            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
