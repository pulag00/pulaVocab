using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pulaVocab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialVocabularySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vocabulary_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    term = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    language = table.Column<int>(type: "integer", nullable: false),
                    part_of_speech = table.Column<int>(type: "integer", nullable: true),
                    cefr_level = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    pronunciation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    personal_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    source = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_entries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "english_word_details",
                columns: table => new
                {
                    vocabulary_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    past_tense = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    past_participle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    third_person_singular = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gerund = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_irregular_verb = table.Column<bool>(type: "boolean", nullable: false),
                    related_phrasal_verbs = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_english_word_details", x => x.vocabulary_entry_id);
                    table.ForeignKey(
                        name: "FK_english_word_details_vocabulary_entries_vocabulary_entry_id",
                        column: x => x.vocabulary_entry_id,
                        principalTable: "vocabulary_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "example_sentences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vocabulary_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sentence = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    translation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_generated_by_ai = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_example_sentences", x => x.id);
                    table.ForeignKey(
                        name: "FK_example_sentences_vocabulary_entries_vocabulary_entry_id",
                        column: x => x.vocabulary_entry_id,
                        principalTable: "vocabulary_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "german_word_details",
                columns: table => new
                {
                    vocabulary_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: true),
                    article = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    plural = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    auxiliary_verb = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    past_participle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_separable_verb = table.Column<bool>(type: "boolean", nullable: false),
                    separable_prefix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    governing_case = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_german_word_details", x => x.vocabulary_entry_id);
                    table.ForeignKey(
                        name: "FK_german_word_details_vocabulary_entries_vocabulary_entry_id",
                        column: x => x.vocabulary_entry_id,
                        principalTable: "vocabulary_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_meanings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vocabulary_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    translation = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    definition = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    context = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_vocabulary_meanings_vocabulary_entries_vocabulary_entry_id",
                        column: x => x.vocabulary_entry_id,
                        principalTable: "vocabulary_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_entry_tags",
                columns: table => new
                {
                    vocabulary_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vocabulary_tag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabulary_entry_tags", x => new { x.vocabulary_entry_id, x.vocabulary_tag_id });
                    table.ForeignKey(
                        name: "FK_vocabulary_entry_tags_vocabulary_entries_vocabulary_entry_id",
                        column: x => x.vocabulary_entry_id,
                        principalTable: "vocabulary_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vocabulary_entry_tags_vocabulary_tags_vocabulary_tag_id",
                        column: x => x.vocabulary_tag_id,
                        principalTable: "vocabulary_tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_example_sentences_vocabulary_entry_id",
                table: "example_sentences",
                column: "vocabulary_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_language",
                table: "vocabulary_entries",
                column: "language");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_status",
                table: "vocabulary_entries",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_term",
                table: "vocabulary_entries",
                column: "term");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_user_id",
                table: "vocabulary_entries",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_entries_user_language_term",
                table: "vocabulary_entries",
                columns: new[] { "user_id", "language", "term" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vocabulary_entry_tags_vocabulary_tag_id",
                table: "vocabulary_entry_tags",
                column: "vocabulary_tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_vocabulary_meanings_vocabulary_entry_id",
                table: "vocabulary_meanings",
                column: "vocabulary_entry_id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "english_word_details");

            migrationBuilder.DropTable(
                name: "example_sentences");

            migrationBuilder.DropTable(
                name: "german_word_details");

            migrationBuilder.DropTable(
                name: "vocabulary_entry_tags");

            migrationBuilder.DropTable(
                name: "vocabulary_meanings");

            migrationBuilder.DropTable(
                name: "vocabulary_tags");

            migrationBuilder.DropTable(
                name: "vocabulary_entries");
        }
    }
}
