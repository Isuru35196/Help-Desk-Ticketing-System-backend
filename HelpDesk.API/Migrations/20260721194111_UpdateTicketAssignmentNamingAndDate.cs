using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketAssignmentNamingAndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_AssignedToUserId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "Tickets",
                newName: "AssignedSupportId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_AssignedToUserId",
                table: "Tickets",
                newName: "IX_Tickets_AssignedSupportId");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignmentDate",
                table: "Tickets",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_AssignedSupportId",
                table: "Tickets",
                column: "AssignedSupportId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_AssignedSupportId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignmentDate",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "AssignedSupportId",
                table: "Tickets",
                newName: "AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_AssignedSupportId",
                table: "Tickets",
                newName: "IX_Tickets_AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_AssignedToUserId",
                table: "Tickets",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
