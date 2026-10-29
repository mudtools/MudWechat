using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DraftRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WechatMediaId = table.Column<string>(type: "TEXT", nullable: false),
                    ItemIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: true),
                    Digest = table.Column<string>(type: "TEXT", nullable: true),
                    ContentHtml = table.Column<string>(type: "TEXT", nullable: true),
                    ContentSourceUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbMediaId = table.Column<string>(type: "TEXT", nullable: true),
                    NeedOpenComment = table.Column<int>(type: "INTEGER", nullable: true),
                    OnlyFansCanComment = table.Column<int>(type: "INTEGER", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRevisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublishId = table.Column<string>(type: "TEXT", nullable: false),
                    WechatMediaId = table.Column<string>(type: "TEXT", nullable: false),
                    LatestStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    ArticleId = table.Column<string>(type: "TEXT", nullable: true),
                    FailIdxJson = table.Column<string>(type: "TEXT", nullable: true),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Attempts = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishStatusSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublishJobId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    PolledAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AttemptNo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishStatusSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishStatusSnapshots_PublishJobs_PublishJobId",
                        column: x => x.PublishJobId,
                        principalTable: "PublishJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DraftRevisions_WechatMediaId_ItemIndex_RevisionNo",
                table: "DraftRevisions",
                columns: new[] { "WechatMediaId", "ItemIndex", "RevisionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_ArticleId",
                table: "PublishJobs",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_PublishId",
                table: "PublishJobs",
                column: "PublishId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublishStatusSnapshots_PublishJobId",
                table: "PublishStatusSnapshots",
                column: "PublishJobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftRevisions");

            migrationBuilder.DropTable(
                name: "PublishStatusSnapshots");

            migrationBuilder.DropTable(
                name: "PublishJobs");
        }
    }
}
