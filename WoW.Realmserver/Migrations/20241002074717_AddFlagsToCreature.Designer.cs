﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WoW.Realmserver.DB;

#nullable disable

namespace WoW.Realmserver.Migrations
{
    [DbContext(typeof(RealmContext))]
    [Migration("20241002074717_AddFlagsToCreature")]
    partial class AddFlagsToCreature
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("BehaviorId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DisplayId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("FlagsLiteral")
                        .HasColumnType("int")
                        .HasColumnName("Flags");

                    b.Property<bool>("IsAggressive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsTargetable")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RawId")
                        .HasColumnType("longtext");

                    b.Property<string>("ScriptId")
                        .IsRequired()
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

                    b.Property<string>("MapId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("RaceId")
                        .HasColumnType("int");

                    b.Property<float>("XPosition")
                        .HasColumnType("float");

                    b.Property<float>("YPosition")
                        .HasColumnType("float");

                    b.HasKey("AccountId");

                    b.ToTable("characters");
                });
#pragma warning restore 612, 618
        }
    }
}
