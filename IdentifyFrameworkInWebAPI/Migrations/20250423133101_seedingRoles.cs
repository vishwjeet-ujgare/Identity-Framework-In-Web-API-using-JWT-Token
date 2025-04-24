using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentifyFrameworkInWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "31176d57-8b72-4b1e-b45b-ff5918e0a5a6", "2", "Student", "STUDENT" },
                    { "77840cb8-55f1-4ff6-9dfa-5eb8bf6ffa38", "3", "Instructor", "INSTRUCTOR" },
                    { "e7e29609-332e-4707-a164-0f579f6d50d7", "1", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "31176d57-8b72-4b1e-b45b-ff5918e0a5a6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "77840cb8-55f1-4ff6-9dfa-5eb8bf6ffa38");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7e29609-332e-4707-a164-0f579f6d50d7");
        }
    }
}
