using Microsoft.EntityFrameworkCore.Migrations;

namespace Promotion.Migrations
{
    public partial class UpdateTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participantions_episodes_EpisodeId",
                table: "Participantions");

            migrationBuilder.DropForeignKey(
                name: "FK_Participantions_Participants_ParticipantId",
                table: "Participantions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participants",
                table: "Participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participantions",
                table: "Participantions");

            migrationBuilder.RenameTable(
                name: "Participants",
                newName: "participants");

            migrationBuilder.RenameTable(
                name: "Participantions",
                newName: "participations");

            migrationBuilder.RenameIndex(
                name: "IX_Participantions_ParticipantId",
                table: "participations",
                newName: "IX_participations_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Participantions_EpisodeId",
                table: "participations",
                newName: "IX_participations_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_participants",
                table: "participants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_participations",
                table: "participations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_participations_episodes_EpisodeId",
                table: "participations",
                column: "EpisodeId",
                principalTable: "episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_participations_participants_ParticipantId",
                table: "participations",
                column: "ParticipantId",
                principalTable: "participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_participations_episodes_EpisodeId",
                table: "participations");

            migrationBuilder.DropForeignKey(
                name: "FK_participations_participants_ParticipantId",
                table: "participations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_participants",
                table: "participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_participations",
                table: "participations");

            migrationBuilder.RenameTable(
                name: "participants",
                newName: "Participants");

            migrationBuilder.RenameTable(
                name: "participations",
                newName: "Participantions");

            migrationBuilder.RenameIndex(
                name: "IX_participations_ParticipantId",
                table: "Participantions",
                newName: "IX_Participantions_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_participations_EpisodeId",
                table: "Participantions",
                newName: "IX_Participantions_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participants",
                table: "Participants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participantions",
                table: "Participantions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Participantions_episodes_EpisodeId",
                table: "Participantions",
                column: "EpisodeId",
                principalTable: "episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participantions_Participants_ParticipantId",
                table: "Participantions",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
