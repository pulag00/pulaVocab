using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pulaVocab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVocabularyEntryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vocabulary_entries_language_term",
                table: "vocabulary_entries");

            migrationBuilder.AddColumn<string>(
                name: "antonyms",
                table: "vocabulary_entries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipa",
                table: "vocabulary_entries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipa_american",
                table: "vocabulary_entries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipa_british",
                table: "vocabulary_entries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_term",
                table: "vocabulary_entries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "related_terms",
                table: "vocabulary_entries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "synonyms",
                table: "vocabulary_entries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "infinitive",
                table: "english_word_details",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_language_normalized_term",
                table: "vocabulary_entries",
                columns: new[] { "language", "normalized_term" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vocabulary_entries_language_normalized_term",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "antonyms",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "ipa",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "ipa_american",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "ipa_british",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "normalized_term",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "related_terms",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "synonyms",
                table: "vocabulary_entries");

            migrationBuilder.DropColumn(
                name: "infinitive",
                table: "english_word_details");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_language_term",
                table: "vocabulary_entries",
                columns: new[] { "language", "term" },
                unique: true);
        }
    }
}
