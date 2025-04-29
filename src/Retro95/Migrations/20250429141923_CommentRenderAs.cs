using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retro95.Migrations
{
    /// <inheritdoc />
    public partial class CommentRenderAs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RenderAs",
                table: "Comment",
                type: "TEXT",
                nullable: false,
                defaultValue: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenderAs",
                table: "Comment");
        }
    }
}
