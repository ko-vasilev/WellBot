﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WellBot.Infrastructure.DataAccess;

namespace WellBot.Infrastructure.DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211220144648_ReplyOptions")]
    partial class ReplyOptions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LoginProvider")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("TelegramId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TelegramId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.ChatData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DataType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileId")
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<bool>("HasUserMention")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("Key");

                    b.ToTable("ChatDatas");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.ChatPidor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("GameDay")
                        .HasColumnType("date");

                    b.Property<int>("RegistrationId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UsedMessageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("RegistrationId");

                    b.HasIndex("UsedMessageId");

                    b.ToTable("ChatPidors");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.PassiveReplyOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DataType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileId")
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PassiveReplyOptions");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.PidorMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DayOfWeek")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("GameDay")
                        .HasColumnType("date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MessageRaw")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<long?>("TelegramUserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Weight")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PidorResultMessages");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.PidorRegistration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalTelegramUserName")
                        .HasMaxLength(63)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<long>("TelegramUserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("PidorRegistrations");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.SlapOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileId")
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SlapOptions");
                });

            modelBuilder.Entity("WellBot.Domain.Users.Entities.AppIdentityRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("WellBot.Domain.Users.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastTokenResetAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("WellBot.Domain.Users.Entities.AppIdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("WellBot.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("WellBot.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("WellBot.Domain.Users.Entities.AppIdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WellBot.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("WellBot.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.ChatData", b =>
                {
                    b.HasOne("WellBot.Domain.Chats.Entities.Chat", "Chat")
                        .WithMany("Data")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.ChatPidor", b =>
                {
                    b.HasOne("WellBot.Domain.Chats.Entities.Chat", "Chat")
                        .WithMany("Pidors")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WellBot.Domain.Chats.Entities.PidorRegistration", "Registration")
                        .WithMany("Wins")
                        .HasForeignKey("RegistrationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WellBot.Domain.Chats.Entities.PidorMessage", "UsedMessage")
                        .WithMany("UsedWins")
                        .HasForeignKey("UsedMessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Registration");

                    b.Navigation("UsedMessage");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.PidorRegistration", b =>
                {
                    b.HasOne("WellBot.Domain.Chats.Entities.Chat", "Chat")
                        .WithMany("PidorRegistrations")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.Chat", b =>
                {
                    b.Navigation("Data");

                    b.Navigation("PidorRegistrations");

                    b.Navigation("Pidors");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.PidorMessage", b =>
                {
                    b.Navigation("UsedWins");
                });

            modelBuilder.Entity("WellBot.Domain.Chats.Entities.PidorRegistration", b =>
                {
                    b.Navigation("Wins");
                });
#pragma warning restore 612, 618
        }
    }
}
