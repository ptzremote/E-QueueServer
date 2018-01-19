using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace QueueServerLib.Migrations
{
    public partial class AddOperatorInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperatorInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OperatorFIO = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueueClientInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientNumber = table.Column<int>(nullable: false),
                    CompleteServiceTime = table.Column<DateTime>(nullable: true),
                    DequeueTime = table.Column<DateTime>(nullable: true),
                    EnqueueTime = table.Column<DateTime>(nullable: false),
                    OperatorInfoId = table.Column<int>(nullable: true),
                    ServiceInfoId = table.Column<int>(nullable: true),
                    WindowNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueClientInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueClientInfo_OperatorInfo_OperatorInfoId",
                        column: x => x.OperatorInfoId,
                        principalTable: "OperatorInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QueueClientInfo_ServiceInfo_ServiceInfoId",
                        column: x => x.ServiceInfoId,
                        principalTable: "ServiceInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueueClientInfo_OperatorInfoId",
                table: "QueueClientInfo",
                column: "OperatorInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueClientInfo_ServiceInfoId",
                table: "QueueClientInfo",
                column: "ServiceInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueClientInfo");

            migrationBuilder.DropTable(
                name: "OperatorInfo");

            migrationBuilder.DropTable(
                name: "ServiceInfo");
        }
    }
}
