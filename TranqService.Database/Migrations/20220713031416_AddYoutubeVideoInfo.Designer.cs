﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TranqService.Database;

#nullable disable

namespace TranqService.Database.Migrations
{
    [DbContext(typeof(TranqDbContext))]
    [Migration("20220713031416_AddYoutubeVideoInfo")]
    partial class AddYoutubeVideoInfo
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("TranqService.Database.Models.YoutubeVideoInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlaylistGuid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Uploader")
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoGuid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("VideoGuid", "PlaylistGuid")
                        .IsUnique();

                    b.ToTable("YoutubeVideoInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
