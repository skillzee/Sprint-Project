using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelApp.Services.Hotel.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipAndApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Approved");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Approved");

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApprovalStatus", "OwnerEmail", "OwnerId", "OwnerName", "RejectionReason" },
                values: new object[] { "Approved", "", 0, "", null });

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApprovalStatus", "OwnerEmail", "OwnerId", "OwnerName", "RejectionReason" },
                values: new object[] { "Approved", "", 0, "", null });

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApprovalStatus", "OwnerEmail", "OwnerId", "OwnerName", "RejectionReason" },
                values: new object[] { "Approved", "", 0, "", null });

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApprovalStatus", "OwnerEmail", "OwnerId", "OwnerName", "RejectionReason" },
                values: new object[] { "Approved", "", 0, "", null });

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApprovalStatus", "OwnerEmail", "OwnerId", "OwnerName", "RejectionReason" },
                values: new object[] { "Approved", "", 0, "", null });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7,
                column: "ApprovalStatus",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8,
                column: "ApprovalStatus",
                value: "Approved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Hotels");
        }
    }
}
