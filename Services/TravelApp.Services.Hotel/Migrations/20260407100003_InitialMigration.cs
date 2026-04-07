using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelApp.Services.Hotel.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StarRating = table.Column<double>(type: "float", nullable: false),
                    Amenities = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxOccupancy = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "Amenities", "City", "Description", "Name", "StarRating" },
                values: new object[,]
                {
                    { 1, "2 Sardar Patel Marg, New Delhi", "Pool,Spa,Gym,Restaurant,Bar,WiFi", "Delhi", "Iconic luxury hotel in the heart of Delhi", "The Taj Palace", 5.0 },
                    { 2, "Marine Lines, Mumbai", "Pool,Restaurant,WiFi,Gym", "Mumbai", "Stunning sea-view suites in South Mumbai", "Marina Bay Suites", 4.0 },
                    { 3, "Calangute Beach, North Goa", "Pool,Beach Access,Spa,Restaurant,Bar,WiFi", "Goa", "Beachfront resort with lush tropical gardens", "Spice Garden Resort", 4.0 },
                    { 4, "Old Manali Road, Manali", "Restaurant,WiFi,Bonfire", "Manali", "Cozy mountain hotel with panoramic Himalayan views", "Himalayan Retreat", 3.0 },
                    { 5, "Pink City, Jaipur", "Pool,Spa,Restaurant,Cultural Shows,WiFi", "Jaipur", "Traditional Rajasthani haveli turned luxury hotel", "Heritage Haveli", 5.0 }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Description", "HotelId", "IsAvailable", "MaxOccupancy", "PricePerNight", "Type" },
                values: new object[,]
                {
                    { 1, "Spacious room with city view", 1, true, 2, 12000m, "Deluxe King" },
                    { 2, "Top-floor suite with panoramic Delhi views", 1, true, 4, 45000m, "Presidential Suite" },
                    { 3, "Room with stunning Arabian Sea views", 2, true, 2, 8500m, "Sea View Double" },
                    { 4, "Comfortable twin room", 2, true, 2, 5500m, "Standard Twin" },
                    { 5, "Private cottage steps from the beach", 3, true, 2, 9000m, "Beach Cottage" },
                    { 6, "Suite overlooking tropical gardens", 3, true, 3, 6500m, "Garden Suite" },
                    { 7, "Cozy room with Himalayan vistas", 4, true, 2, 3500m, "Mountain View Room" },
                    { 8, "Opulent Rajasthani-themed royal suite", 5, true, 2, 18000m, "Royal Suite" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
