using Microsoft.EntityFrameworkCore.Migrations;

namespace Planiture_Website.Migrations
{
    public partial class livechatupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigFiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AgentPass = table.Column<string>(nullable: true),
                    AdminPass = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigFiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigFiles");
        }
    }
}
