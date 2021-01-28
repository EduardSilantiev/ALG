﻿// <auto-generated />
using System;
using ALG.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ALG.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(AlgDbContext))]
    [Migration("20210127203658_Initial migration")]
    partial class Initialmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("onsi")
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("ALG.Core.Services.ActivatedBonus", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ServiceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "ServiceId")
                        .HasName("PK_UserService");

                    b.HasIndex("ServiceId");

                    b.ToTable("ActivatedBonuses");
                });

            modelBuilder.Entity("ALG.Core.Services.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Promocode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Service");
                });

            modelBuilder.Entity("ALG.Core.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("ALG.Core.Services.ActivatedBonus", b =>
                {
                    b.HasOne("ALG.Core.Services.Service", "Service")
                        .WithMany("ActivatedBonuses")
                        .HasForeignKey("ServiceId")
                        .HasConstraintName("FK_ActivatedBonus_Service")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ALG.Core.Users.User", "User")
                        .WithMany("ActivatedBonuses")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_ActivatedBonus_User")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ALG.Core.Services.Service", b =>
                {
                    b.Navigation("ActivatedBonuses");
                });

            modelBuilder.Entity("ALG.Core.Users.User", b =>
                {
                    b.Navigation("ActivatedBonuses");
                });
#pragma warning restore 612, 618
        }
    }
}
