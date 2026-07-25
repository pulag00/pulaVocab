using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pulaVocab.Infrastructure.Migrations;

public partial class RemoveUserOwnership : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_vocabulary_entries_user_id", table: "vocabulary_entries");
        migrationBuilder.DropIndex(name: "ix_vocabulary_entries_user_language_term", table: "vocabulary_entries");
        migrationBuilder.DropIndex(name: "ix_vocabulary_tags_user_id", table: "vocabulary_tags");
        migrationBuilder.DropIndex(name: "ix_vocabulary_tags_user_name", table: "vocabulary_tags");
        migrationBuilder.DropIndex(name: "ix_vocabulary_tags_name", table: "vocabulary_tags");
        migrationBuilder.DropColumn(name: "user_id", table: "vocabulary_entries");
        migrationBuilder.DropColumn(name: "user_id", table: "vocabulary_tags");
        migrationBuilder.CreateIndex(name: "ix_vocabulary_entries_language_term", table: "vocabulary_entries", columns: new[] { "language", "term" }, unique: true);
        migrationBuilder.CreateIndex(name: "ix_vocabulary_tags_name", table: "vocabulary_tags", column: "name", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_vocabulary_entries_language_term", table: "vocabulary_entries");
        migrationBuilder.DropIndex(name: "ix_vocabulary_tags_name", table: "vocabulary_tags");
        migrationBuilder.CreateIndex(name: "ix_vocabulary_tags_name", table: "vocabulary_tags", column: "name");
        migrationBuilder.AddColumn<Guid>(name: "user_id", table: "vocabulary_entries", type: "uuid", nullable: false, defaultValue: Guid.Empty);
        migrationBuilder.AddColumn<Guid>(name: "user_id", table: "vocabulary_tags", type: "uuid", nullable: false, defaultValue: Guid.Empty);
        migrationBuilder.CreateIndex(name: "ix_vocabulary_entries_user_id", table: "vocabulary_entries", column: "user_id");
        migrationBuilder.CreateIndex(name: "ix_vocabulary_entries_user_language_term", table: "vocabulary_entries", columns: new[] { "user_id", "language", "term" }, unique: true);
        migrationBuilder.CreateIndex(name: "ix_vocabulary_tags_user_id", table: "vocabulary_tags", column: "user_id");
        migrationBuilder.CreateIndex(name: "ix_vocabulary_tags_user_name", table: "vocabulary_tags", columns: new[] { "user_id", "name" }, unique: true);
    }
}
