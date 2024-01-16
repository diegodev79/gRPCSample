using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPCSampleApp.Migrations
{
    public partial class UpdateAuctionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatorId",
                table: "AuctionItems");

            migrationBuilder.AddColumn<string>(
                name: "InitiatorName",
                table: "AuctionItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatorName",
                table: "AuctionItems");

            migrationBuilder.AddColumn<int>(
                name: "InitiatorId",
                table: "AuctionItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
