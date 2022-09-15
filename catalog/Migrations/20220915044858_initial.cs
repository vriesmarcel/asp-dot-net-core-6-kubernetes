using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloboTicket.Catalog.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Concerts",
                columns: table => new
                {
                    ConcertId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concerts", x => x.ConcertId);
                });

            migrationBuilder.InsertData(
                table: "Concerts",
                columns: new[] { "ConcertId", "Artist", "Date", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("3448d5a4-0f72-4dd7-bf15-c14a46b26c00"), "Michael Johnson", new DateTime(2023, 6, 15, 6, 48, 58, 835, DateTimeKind.Local).AddTicks(9806), "Michael Johnson doesn't need an introduction. His 25 concert across the globe last year were seen by thousands. Can we add you to the list?", "https://gillcleerenpluralsight.blob.core.windows.net/files/GloboTicket/michael.jpg", "The State of Affairs: Michael Live!", 85 },
                    { new Guid("62787623-4c52-43fe-b0c9-b7044fb5929b"), "Manuel Santinonisi", new DateTime(2023, 1, 15, 6, 48, 58, 835, DateTimeKind.Local).AddTicks(9849), "Get on the hype of Spanish Guitar concerts with Manuel.", "https://gillcleerenpluralsight.blob.core.windows.net/files/GloboTicket/guitar.jpg", "Spanish guitar hits with Manuel", 25 },
                    { new Guid("adc42c09-08c1-4d2c-9f96-2d15bb1af299"), "Nick Sailor", new DateTime(2023, 5, 15, 6, 48, 58, 835, DateTimeKind.Local).AddTicks(9857), "The critics are over the moon and so will you after you've watched this sing and dance extravaganza written by Nick Sailor, the man from 'My dad and sister'.", "/img/musical.jpg", "To the Moon and Back", 135 },
                    { new Guid("b419a7ca-3321-4f38-be8e-4d7b6a529319"), "DJ 'The Mike'", new DateTime(2023, 1, 15, 6, 48, 58, 835, DateTimeKind.Local).AddTicks(9815), "DJs from all over the world will compete in this epic battle for eternal fame.", "https://gillcleerenpluralsight.blob.core.windows.net/files/GloboTicket/dj.jpg", "Clash of the DJs", 85 },
                    { new Guid("ee272f8b-6096-4cb6-8625-bb4bb2d89e8b"), "John Egbert", new DateTime(2023, 3, 15, 6, 48, 58, 835, DateTimeKind.Local).AddTicks(9742), "Join John for his farwell tour across 15 continents. John really needs no introduction since he has already mesmerized the world with his banjo.", "https://gillcleerenpluralsight.blob.core.windows.net/files/GloboTicket/banjo.jpg", "John Egbert Live", 65 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Concerts");
        }
    }
}
