using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financier.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.BudgetId);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SubType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountRelationships",
                columns: table => new
                {
                    AccountRelationshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceAccountId = table.Column<int>(type: "int", nullable: false),
                    DestinationAccountId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRelationships", x => x.AccountRelationshipId);
                    table.ForeignKey(
                        name: "FK_AccountRelationships_Accounts_DestinationAccountId",
                        column: x => x.DestinationAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountRelationships_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetTransactions",
                columns: table => new
                {
                    BudgetTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsInitial = table.Column<bool>(type: "bit", nullable: false),
                    IsSurplus = table.Column<bool>(type: "bit", nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetTransactions", x => x.BudgetTransactionId);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionRelationships",
                columns: table => new
                {
                    TransactionRelationshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceTransactionId = table.Column<int>(type: "int", nullable: false),
                    DestinationTransactionId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRelationships", x => x.TransactionRelationshipId);
                    table.ForeignKey(
                        name: "FK_TransactionRelationships_Transactions_DestinationTransactionId",
                        column: x => x.DestinationTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionRelationships_Transactions_SourceTransactionId",
                        column: x => x.SourceTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRelationships_DestinationAccountId",
                table: "AccountRelationships",
                column: "DestinationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRelationships_SourceAccountId",
                table: "AccountRelationships",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_BudgetId",
                table: "BudgetTransactions",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_CreditAccountId",
                table: "BudgetTransactions",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_DebitAccountId",
                table: "BudgetTransactions",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionRelationships_DestinationTransactionId",
                table: "TransactionRelationships",
                column: "DestinationTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionRelationships_SourceTransactionId",
                table: "TransactionRelationships",
                column: "SourceTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditAccountId",
                table: "Transactions",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DebitAccountId",
                table: "Transactions",
                column: "DebitAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRelationships");

            migrationBuilder.DropTable(
                name: "BudgetTransactions");

            migrationBuilder.DropTable(
                name: "TransactionRelationships");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
