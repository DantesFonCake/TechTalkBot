﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TechTalkBot.Database;

#nullable disable

namespace TechTalkBot.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231027101121_unique")]
    partial class unique
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PollVideo", b =>
                {
                    b.Property<long>("OptionsId")
                        .HasColumnType("bigint");

                    b.Property<int>("PollId")
                        .HasColumnType("integer");

                    b.HasKey("OptionsId", "PollId");

                    b.HasIndex("PollId");

                    b.ToTable("PollVideo");
                });

            modelBuilder.Entity("TechTalkBot.Database.Chat", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int?>("ActivePollId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ActivePollId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("TechTalkBot.Database.Poll", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("EndedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("WinnerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("WinnerId");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("TechTalkBot.Database.Video", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("WasInPoll")
                        .HasColumnType("boolean");

                    b.Property<bool>("Watched")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Url", "Name")
                        .IsUnique();

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("PollVideo", b =>
                {
                    b.HasOne("TechTalkBot.Database.Video", null)
                        .WithMany()
                        .HasForeignKey("OptionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TechTalkBot.Database.Poll", null)
                        .WithMany()
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TechTalkBot.Database.Chat", b =>
                {
                    b.HasOne("TechTalkBot.Database.Poll", "ActivePoll")
                        .WithMany()
                        .HasForeignKey("ActivePollId");

                    b.Navigation("ActivePoll");
                });

            modelBuilder.Entity("TechTalkBot.Database.Poll", b =>
                {
                    b.HasOne("TechTalkBot.Database.Video", "Winner")
                        .WithMany()
                        .HasForeignKey("WinnerId");

                    b.Navigation("Winner");
                });
#pragma warning restore 612, 618
        }
    }
}