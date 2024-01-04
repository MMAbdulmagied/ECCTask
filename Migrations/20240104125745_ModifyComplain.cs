using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class ModifyComplain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Complains");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Complains");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Complains",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TitleAr",
                table: "Complains",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Complains",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Complains",
                newName: "TitleAr");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Complains",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Complains",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
