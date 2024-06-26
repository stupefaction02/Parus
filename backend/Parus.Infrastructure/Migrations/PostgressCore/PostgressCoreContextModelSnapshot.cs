﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Parus.Infrastructure.DLA;

#nullable disable

namespace Parus.Infrastructure.Migrations.PostgressCore
{
    [DbContext(typeof(PostgressCoreContext))]
    partial class PostgressCoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BroadcastInfoTag", b =>
                {
                    b.Property<int>("BroadcastsId")
                        .HasColumnType("integer");

                    b.Property<int>("TagsId")
                        .HasColumnType("integer");

                    b.HasKey("BroadcastsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("BroadcastInfoTag");
                });

            modelBuilder.Entity("Parus.Core.Entities.BroadcastCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte>("IndexingStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((byte)1);

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.Property<int>("ViewsCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Parus.Core.Entities.BroadcastInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AvatarPic")
                        .HasColumnType("text");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("HostUserId")
                        .HasColumnType("text");

                    b.Property<byte>("IndexingStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((byte)1);

                    b.Property<string>("Preview")
                        .HasColumnType("text");

                    b.Property<string>("Ref")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Broadcasts");
                });

            modelBuilder.Entity("Parus.Core.Entities.BroadcastInfoKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BroadcastInfoId")
                        .HasColumnType("integer");

                    b.Property<string>("Keyword")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BroadcastInfoId");

                    b.ToTable("BroadcastsKeywords");
                });

            modelBuilder.Entity("Parus.Core.Entities.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("text");

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("Parus.Core.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("BroadcastInfoTag", b =>
                {
                    b.HasOne("Parus.Core.Entities.BroadcastInfo", null)
                        .WithMany()
                        .HasForeignKey("BroadcastsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parus.Core.Entities.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Parus.Core.Entities.BroadcastInfo", b =>
                {
                    b.HasOne("Parus.Core.Entities.BroadcastCategory", "Category")
                        .WithMany("Broadcasts")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Parus.Core.Entities.BroadcastInfoKeyword", b =>
                {
                    b.HasOne("Parus.Core.Entities.BroadcastInfo", "BroadcastInfo")
                        .WithMany()
                        .HasForeignKey("BroadcastInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BroadcastInfo");
                });

            modelBuilder.Entity("Parus.Core.Entities.BroadcastCategory", b =>
                {
                    b.Navigation("Broadcasts");
                });
#pragma warning restore 612, 618
        }
    }
}
