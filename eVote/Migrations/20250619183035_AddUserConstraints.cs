using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote.Migrations
{
    /// <inheritdoc />
    public partial class AddUserConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Vote_VoteId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Users_UserId",
                table: "Vote");

            migrationBuilder.DropIndex(
                name: "IX_Users_VoteId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vote",
                table: "Vote");

            migrationBuilder.DropIndex(
                name: "IX_Vote_UserId",
                table: "Vote");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Vote");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Vote");

            migrationBuilder.RenameTable(
                name: "Vote",
                newName: "Votes");

            migrationBuilder.RenameColumn(
                name: "VoteId",
                table: "Users",
                newName: "Password");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                columns: new[] { "VoterId", "CandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VoterId_CandidateId",
                table: "Votes",
                columns: new[] { "VoterId", "CandidateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VoterId_CandidateId",
                table: "Votes");

            migrationBuilder.RenameTable(
                name: "Votes",
                newName: "Vote");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "VoteId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Vote",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Vote",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vote",
                table: "Vote",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_VoteId",
                table: "Users",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_UserId",
                table: "Vote",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Vote_VoteId",
                table: "Users",
                column: "VoteId",
                principalTable: "Vote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Users_UserId",
                table: "Vote",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
