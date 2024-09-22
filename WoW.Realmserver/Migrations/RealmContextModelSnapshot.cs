﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WoW.Realmserver.DB;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    [DbContext(typeof(RealmContext))]
    partial class RealmContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("WoW.Realmserver.DB.Model.Creature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("FlagsLiteral")
                        .HasColumnType("int")
                        .HasColumnName("Flags");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RawId")
                        .HasColumnType("longtext");

                    b.Property<string>("SubName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("creatures");
                });

            modelBuilder.Entity("WoW.Realmserver.DB.Model.PlayerCharacter", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("ClassId")
                        .HasColumnType("int");

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("RaceId")
                        .HasColumnType("int");

                    b.HasKey("AccountId");

                    b.ToTable("characters");
                });
#pragma warning restore 612, 618
        }
    }
}
