using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quizard.Infrastructure.DataProvider.DbContext.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsRunning = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ScenarioId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenseSecrets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseSecrets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    MediaId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    DelaySeconds = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    ShowOnPlayer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.MediaId);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    Nickname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoundTypes",
                columns: table => new
                {
                    RoundTypeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RoundClass = table.Column<int>(type: "integer", nullable: false),
                    WaitingRoundTimeout = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayQuestionOnInformator = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayQuestionOnPlayers = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundTypes", x => x.RoundTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    GameDuration = table.Column<int>(type: "integer", nullable: true),
                    BasePointPrice = table.Column<int>(type: "integer", nullable: false),
                    StartPlayerScore = table.Column<int>(type: "integer", nullable: false),
                    RoundPresentationDuration = table.Column<int>(type: "integer", nullable: true),
                    ShowScenarioStatsOnFinish = table.Column<bool>(type: "boolean", nullable: false),
                    FinishPlaceholder = table.Column<string>(type: "text", nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseKey = table.Column<string>(type: "text", nullable: false),
                    ExpirationTime = table.Column<string>(type: "text", nullable: false),
                    GamesLeft = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    SaltId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Licenses_LicenseSecrets_SaltId",
                        column: x => x.SaltId,
                        principalTable: "LicenseSecrets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Localizations",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    ScenarioId = table.Column<string>(type: "text", nullable: false),
                    ButtonUp = table.Column<string>(type: "text", nullable: false),
                    ButtonDown = table.Column<string>(type: "text", nullable: false),
                    ButtonConfirm = table.Column<string>(type: "text", nullable: false),
                    ButtonCancel = table.Column<string>(type: "text", nullable: false),
                    ButtonContinue = table.Column<string>(type: "text", nullable: false),
                    ButtonBuy = table.Column<string>(type: "text", nullable: false),
                    ButtonFinish = table.Column<string>(type: "text", nullable: false),
                    ButtonStub = table.Column<string>(type: "text", nullable: false),
                    BidButtonLabel = table.Column<string>(type: "text", nullable: false),
                    AttentionLabel = table.Column<string>(type: "text", nullable: false),
                    WaitOtherPlayersLabel = table.Column<string>(type: "text", nullable: false),
                    WaitYourTurnLabel = table.Column<string>(type: "text", nullable: false),
                    HintLabel = table.Column<string>(type: "text", nullable: false),
                    SpeedWinnerLabel = table.Column<string>(type: "text", nullable: false),
                    PantomimeLabel = table.Column<string>(type: "text", nullable: false),
                    VotingLabel = table.Column<string>(type: "text", nullable: false),
                    PlayerLabel = table.Column<string>(type: "text", nullable: false),
                    ScoreLabel = table.Column<string>(type: "text", nullable: false),
                    BetTime = table.Column<string>(type: "text", nullable: false),
                    MakeBetLabelPart1 = table.Column<string>(type: "text", nullable: false),
                    MakeBetLabelPart2 = table.Column<string>(type: "text", nullable: false),
                    BidAcceptedLabelPart1 = table.Column<string>(type: "text", nullable: false),
                    BidAcceptedLabelPart2 = table.Column<string>(type: "text", nullable: false),
                    PriceLabel = table.Column<string>(type: "text", nullable: false),
                    LoginLabel = table.Column<string>(type: "text", nullable: false),
                    RoundLabel = table.Column<string>(type: "text", nullable: false),
                    RoundResultsLabel = table.Column<string>(type: "text", nullable: false),
                    RoundChoiceLabel = table.Column<string>(type: "text", nullable: false),
                    ShopLabel = table.Column<string>(type: "text", nullable: false),
                    InsufficientFunds = table.Column<string>(type: "text", nullable: false),
                    ProductHasBeenPurchased = table.Column<string>(type: "text", nullable: false),
                    ProductAlreadyPurchased = table.Column<string>(type: "text", nullable: false),
                    ShopWaitLabel = table.Column<string>(type: "text", nullable: false),
                    ApplyingModifiersLabel = table.Column<string>(type: "text", nullable: false),
                    GameFinishLabel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localizations_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    RoundId = table.Column<string>(type: "text", nullable: false),
                    ScenarioId = table.Column<string>(type: "text", nullable: false),
                    RoundTypeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PreviewUrl = table.Column<string>(type: "text", nullable: true),
                    RoundDuration = table.Column<int>(type: "integer", nullable: true),
                    CorrectMultiplier = table.Column<int>(type: "integer", nullable: false),
                    MissedMultiplier = table.Column<int>(type: "integer", nullable: false),
                    IncorrectMultiplier = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.RoundId);
                    table.ForeignKey(
                        name: "FK_Rounds_RoundTypes_RoundTypeId",
                        column: x => x.RoundTypeId,
                        principalTable: "RoundTypes",
                        principalColumn: "RoundTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rounds_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioStatistics",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    GameId = table.Column<byte[]>(type: "bytea", nullable: false),
                    PlayerId = table.Column<byte[]>(type: "bytea", nullable: false),
                    ScenarioId = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioStatistics_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioStatistics_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioStatistics_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    ScenarioId = table.Column<string>(type: "text", nullable: false),
                    ModifierType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CostMultiplier = table.Column<int>(type: "integer", nullable: false),
                    UniqForPlayer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    RoundId = table.Column<string>(type: "text", nullable: false),
                    QuestionNumber = table.Column<int>(type: "integer", nullable: false),
                    QuestionText = table.Column<string>(type: "text", nullable: false),
                    MediaId = table.Column<string>(type: "text", nullable: true),
                    AnswerDelay = table.Column<int>(type: "integer", nullable: true),
                    QuestionTimeout = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoundStatistics",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    GameId = table.Column<byte[]>(type: "bytea", nullable: false),
                    PlayerId = table.Column<byte[]>(type: "bytea", nullable: false),
                    RoundId = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoundStatistics_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoundStatistics_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoundStatistics_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    ScenarioId = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<string>(type: "text", nullable: true),
                    RoundId = table.Column<string>(type: "text", nullable: true),
                    StageDuration = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stages_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stages_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stages_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    QuestionId = table.Column<byte[]>(type: "bytea", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Button = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hints",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    QuestionId = table.Column<byte[]>(type: "bytea", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hints_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reveals",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "bytea", nullable: false),
                    QuestionId = table.Column<byte[]>(type: "bytea", nullable: false),
                    MediaId = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reveals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reveals_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reveals_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Hints_QuestionId",
                table: "Hints",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_SaltId",
                table: "Licenses",
                column: "SaltId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localizations_ScenarioId",
                table: "Localizations",
                column: "ScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Nickname",
                table: "Players",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_MediaId",
                table: "Questions",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_RoundId",
                table: "Questions",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Reveals_MediaId",
                table: "Reveals",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reveals_QuestionId",
                table: "Reveals",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_RoundTypeId",
                table: "Rounds",
                column: "RoundTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_ScenarioId",
                table: "Rounds",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundStatistics_GameId",
                table: "RoundStatistics",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundStatistics_PlayerId",
                table: "RoundStatistics",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundStatistics_RoundId",
                table: "RoundStatistics",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioStatistics_GameId",
                table: "ScenarioStatistics",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioStatistics_PlayerId",
                table: "ScenarioStatistics",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioStatistics_ScenarioId",
                table: "ScenarioStatistics",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_MediaId",
                table: "Stages",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_RoundId",
                table: "Stages",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_ScenarioId",
                table: "Stages",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ScenarioId",
                table: "Stocks",
                column: "ScenarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Hints");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "Localizations");

            migrationBuilder.DropTable(
                name: "Reveals");

            migrationBuilder.DropTable(
                name: "RoundStatistics");

            migrationBuilder.DropTable(
                name: "ScenarioStatistics");

            migrationBuilder.DropTable(
                name: "Stages");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "LicenseSecrets");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropTable(
                name: "RoundTypes");

            migrationBuilder.DropTable(
                name: "Scenarios");
        }
    }
}
