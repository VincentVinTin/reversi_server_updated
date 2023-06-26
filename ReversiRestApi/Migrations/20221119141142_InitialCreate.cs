﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversiRestApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spellen",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Omschrijving = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Speler1Token = table.Column<string>(nullable: true),
                    Speler2Token = table.Column<string>(nullable: true),
                    Bord = table.Column<string>(nullable: true),
                    AandeBeurt = table.Column<int>(nullable: false),
                    Winnaar = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spellen", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spellen");
        }
    }
}
