﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TvMazeCollector.DAL;

#nullable disable

namespace TvMazeCollector.DAL.Migrations
{
    [DbContext(typeof(TvMazeDbContext))]
    partial class TvMazeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TvMazeCollector.DAL.Entities.Actor", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Actors");
                });

            modelBuilder.Entity("TvMazeCollector.DAL.Entities.ActorShow", b =>
                {
                    b.Property<int>("ActorId")
                        .HasColumnType("int");

                    b.Property<int>("ShowId")
                        .HasColumnType("int");

                    b.HasKey("ActorId", "ShowId");

                    b.HasIndex("ShowId");

                    b.ToTable("ActorShows");
                });

            modelBuilder.Entity("TvMazeCollector.DAL.Entities.Show", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("TvMazeCollector.DAL.Entities.ActorShow", b =>
                {
                    b.HasOne("TvMazeCollector.DAL.Entities.Actor", "Actor")
                        .WithMany("ActorShows")
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TvMazeCollector.DAL.Entities.Show", "Show")
                        .WithMany("ActorShows")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Actor");

                    b.Navigation("Show");
                });

            modelBuilder.Entity("TvMazeCollector.DAL.Entities.Actor", b =>
                {
                    b.Navigation("ActorShows");
                });

            modelBuilder.Entity("TvMazeCollector.DAL.Entities.Show", b =>
                {
                    b.Navigation("ActorShows");
                });
#pragma warning restore 612, 618
        }
    }
}