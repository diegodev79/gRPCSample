﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RPCSampleApp.Services;

#nullable disable

namespace RPCSampleApp.Migrations
{
    [DbContext(typeof(AuctionDbContext))]
    [Migration("20240116134047_UpdateAuctionItem2")]
    partial class UpdateAuctionItem2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.26");

            modelBuilder.Entity("RPCSampleApp.Models.AuctionItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("InitialPrice")
                        .HasColumnType("INTEGER");

                    b.Property<string>("InitiatorName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AuctionItems");
                });

            modelBuilder.Entity("RPCSampleApp.Models.AuctionNode", b =>
                {
                    b.Property<int>("NodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NodeName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.HasKey("NodeId");

                    b.ToTable("AuctionNodes");
                });

            modelBuilder.Entity("RPCSampleApp.Models.Bid", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuctionItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BidAmount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BidderName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuctionItemId");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("RPCSampleApp.Models.Bid", b =>
                {
                    b.HasOne("RPCSampleApp.Models.AuctionItem", "AuctionItem")
                        .WithMany("Bids")
                        .HasForeignKey("AuctionItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuctionItem");
                });

            modelBuilder.Entity("RPCSampleApp.Models.AuctionItem", b =>
                {
                    b.Navigation("Bids");
                });
#pragma warning restore 612, 618
        }
    }
}