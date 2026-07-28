using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pulaVocab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarModeloVocabulario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vocabulary_tags_name",
                table: "vocabulary_tags");

            migrationBuilder.DropIndex(
                name: "ix_vocabulary_tags_user_id",
                table: "vocabulary_tags");

            migrationBuilder.DropIndex(
                name: "ix_vocabulary_tags_user_name",
                table: "vocabulary_tags");

            migrationBuilder.DropIndex(
                name: "ix_vocabulary_entries_user_id",
                table: "vocabulary_entries");

            migrationBuilder.DropIndex(
                name: "ix_vocabulary_entries_user_language_term",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "vocabulary_tags");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "vocabulary_entries");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_tags_name",
                table: "vocabulary_tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_language_term",
                table: "vocabulary_entries",
                columns: new[] { "language", "term" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vocabulary_tags_name",
                table: "vocabulary_tags");

            migrationBuilder.DropIndex(
                name: "ix_vocabulary_entries_language_term",
                table: "vocabulary_entries");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "vocabulary_tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "vocabulary_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_tags_name",
                table: "vocabulary_tags",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_tags_user_id",
                table: "vocabulary_tags",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_tags_user_name",
                table: "vocabulary_tags",
                columns: new[] { "user_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_user_id",
                table: "vocabulary_entries",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_user_language_term",
                table: "vocabulary_entries",
                columns: new[] { "user_id", "language", "term" },
                unique: true);
        }
    }
}
