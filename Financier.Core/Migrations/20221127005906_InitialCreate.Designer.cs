﻿// <auto-generated />
using System;
using Financier.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Financier.Migrations
{
    [DbContext(typeof(FinancierDbContext))]
    [Migration("20221127005906_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Financier.Entities.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountId"));

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SubType")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("AccountId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Financier.Entities.AccountRelationship", b =>
                {
                    b.Property<int>("AccountRelationshipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountRelationshipId"));

                    b.Property<int>("DestinationAccountId")
                        .HasColumnType("int");

                    b.Property<int>("SourceAccountId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("AccountRelationshipId");

                    b.HasIndex("DestinationAccountId");

                    b.HasIndex("SourceAccountId");

                    b.ToTable("AccountRelationships");
                });

            modelBuilder.Entity("Financier.Entities.Budget", b =>
                {
                    b.Property<int>("BudgetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BudgetId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.HasKey("BudgetId");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("Financier.Entities.BudgetTransaction", b =>
                {
                    b.Property<int>("BudgetTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BudgetTransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BudgetId")
                        .HasColumnType("int");

                    b.Property<int>("CreditAccountId")
                        .HasColumnType("int");

                    b.Property<int>("DebitAccountId")
                        .HasColumnType("int");

                    b.Property<bool>("IsInitial")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSurplus")
                        .HasColumnType("bit");

                    b.HasKey("BudgetTransactionId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("CreditAccountId");

                    b.HasIndex("DebitAccountId");

                    b.ToTable("BudgetTransactions");
                });

            modelBuilder.Entity("Financier.Entities.Currency", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CurrencyId"));

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CurrencyId");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Financier.Entities.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("At")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreditAccountId")
                        .HasColumnType("int");

                    b.Property<int>("DebitAccountId")
                        .HasColumnType("int");

                    b.HasKey("TransactionId");

                    b.HasIndex("CreditAccountId");

                    b.HasIndex("DebitAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Financier.Entities.TransactionRelationship", b =>
                {
                    b.Property<int>("TransactionRelationshipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionRelationshipId"));

                    b.Property<int>("DestinationTransactionId")
                        .HasColumnType("int");

                    b.Property<int>("SourceTransactionId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("TransactionRelationshipId");

                    b.HasIndex("DestinationTransactionId");

                    b.HasIndex("SourceTransactionId");

                    b.ToTable("TransactionRelationships");
                });

            modelBuilder.Entity("Financier.Entities.Account", b =>
                {
                    b.HasOne("Financier.Entities.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("Financier.Entities.AccountRelationship", b =>
                {
                    b.HasOne("Financier.Entities.Account", "DestinationAccount")
                        .WithMany()
                        .HasForeignKey("DestinationAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Financier.Entities.Account", "SourceAccount")
                        .WithMany()
                        .HasForeignKey("SourceAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DestinationAccount");

                    b.Navigation("SourceAccount");
                });

            modelBuilder.Entity("Financier.Entities.BudgetTransaction", b =>
                {
                    b.HasOne("Financier.Entities.Budget", "Budget")
                        .WithMany("Transactions")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Financier.Entities.Account", "CreditAccount")
                        .WithMany()
                        .HasForeignKey("CreditAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Financier.Entities.Account", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Budget");

                    b.Navigation("CreditAccount");

                    b.Navigation("DebitAccount");
                });

            modelBuilder.Entity("Financier.Entities.Transaction", b =>
                {
                    b.HasOne("Financier.Entities.Account", "CreditAccount")
                        .WithMany()
                        .HasForeignKey("CreditAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Financier.Entities.Account", "DebitAccount")
                        .WithMany()
                        .HasForeignKey("DebitAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreditAccount");

                    b.Navigation("DebitAccount");
                });

            modelBuilder.Entity("Financier.Entities.TransactionRelationship", b =>
                {
                    b.HasOne("Financier.Entities.Transaction", "DestinationTransaction")
                        .WithMany()
                        .HasForeignKey("DestinationTransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Financier.Entities.Transaction", "SourceTransaction")
                        .WithMany()
                        .HasForeignKey("SourceTransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DestinationTransaction");

                    b.Navigation("SourceTransaction");
                });

            modelBuilder.Entity("Financier.Entities.Budget", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
