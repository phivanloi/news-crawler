using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pl.Crawler.Data.Migrations.CrawlDbMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrawlConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    WebsiteId = table.Column<long>(nullable: false),
                    PageTypeName = table.Column<string>(maxLength: 128, nullable: false),
                    UrlPattern = table.Column<string>(maxLength: 512, nullable: false),
                    AutoExport = table.Column<bool>(nullable: false),
                    ExportApiUrl = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawlConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParseFields",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    CrawlConfigId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParseFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReplateRules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    ParseFieldId = table.Column<long>(nullable: false),
                    SelectKey = table.Column<string>(nullable: true),
                    ReplaceData = table.Column<string>(nullable: true),
                    ParserType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplateRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelectRules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    ParseFieldId = table.Column<long>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    DefaultData = table.Column<string>(nullable: true),
                    SelectKey = table.Column<string>(nullable: true),
                    ParserType = table.Column<int>(nullable: false),
                    IsHtml = table.Column<bool>(nullable: false),
                    SelectMultiple = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sitemaps",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    WebsiteId = table.Column<long>(nullable: false),
                    Url = table.Column<string>(maxLength: 2048, nullable: false),
                    DownloadRank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sitemaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Websites",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    Domain = table.Column<string>(maxLength: 2048, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    Rank = table.Column<float>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    FindLinkOnlySiteMap = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Websites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Websites_Domain",
                table: "Websites",
                column: "Domain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrawlConfigs");

            migrationBuilder.DropTable(
                name: "ParseFields");

            migrationBuilder.DropTable(
                name: "ReplateRules");

            migrationBuilder.DropTable(
                name: "SelectRules");

            migrationBuilder.DropTable(
                name: "Sitemaps");

            migrationBuilder.DropTable(
                name: "Websites");
        }
    }
}
