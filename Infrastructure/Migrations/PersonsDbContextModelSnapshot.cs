﻿// <auto-generated />
using System;
using Entities;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entities.Migrations
{
    [DbContext(typeof(PersonsDbContext))]
    partial class PersonsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entities.Country", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("ID");

                    b.ToTable("Countries", (string)null);

                    b.HasData(
                        new
                        {
                            ID = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            Name = "Germany"
                        },
                        new
                        {
                            ID = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            Name = "England"
                        },
                        new
                        {
                            ID = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            Name = "USA"
                        },
                        new
                        {
                            ID = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            Name = "Italy"
                        },
                        new
                        {
                            ID = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            Name = "Sweaden"
                        });
                });

            modelBuilder.Entity("Entities.IdentityEntities.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("53253ccd-15c7-4c49-8021-a0f1633899c0"),
                            ConcurrencyStamp = "5050e615-22ca-404a-a319-24b672a7a333",
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = new Guid("87ef14a9-2d76-43b4-9ca2-61f508d9b016"),
                            ConcurrencyStamp = "2ef91154-c51a-46aa-9eb7-84dd64cc6a74",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        });
                });

            modelBuilder.Entity("Entities.IdentityEntities.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Entities.Person", b =>
                {
                    b.Property<Guid?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid>("CountryID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<bool>("ReceiveNewsLetters")
                        .HasColumnType("bit");

                    b.Property<string>("TIN")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)")
                        .HasDefaultValue("ABCDEFGH123")
                        .HasColumnName("TINumber");

                    b.HasKey("ID");

                    b.HasIndex("CountryID");

                    b.ToTable("Persons", null, t =>
                        {
                            t.HasCheckConstraint("CHK_TINumber", "len([TINumber]) = 11");
                        });

                    b.HasData(
                        new
                        {
                            ID = new Guid("c03bbe45-9aeb-4d24-99e0-4743016ffce9"),
                            Address = "4 Parkside Point",
                            CountryID = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            DateOfBirth = new DateTime(1989, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mwebsdale0@people.com.cn",
                            Gender = "Female",
                            Name = "Marguerite",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("c3abddbd-cf50-41d2-b6c4-cc7d5a750928"),
                            Address = "6 Morningstar Circle",
                            CountryID = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            DateOfBirth = new DateTime(1990, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "ushears1@globo.com",
                            Gender = "Female",
                            Name = "Ursa",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("c6d50a47-f7e6-4482-8be0-4ddfc057fa6e"),
                            Address = "73 Heath Avenue",
                            CountryID = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            DateOfBirth = new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "fbowsher2@howstuffworks.com",
                            Gender = "Male",
                            Name = "Franchot",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("d15c6d9f-70b4-48c5-afd3-e71261f1a9be"),
                            Address = "83187 Merry Drive",
                            CountryID = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            DateOfBirth = new DateTime(1987, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "asarvar3@dropbox.com",
                            Gender = "Male",
                            Name = "Angie",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                            Address = "50467 Holy Cross Crossing",
                            CountryID = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            DateOfBirth = new DateTime(1995, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "ttregona4@stumbleupon.com",
                            Gender = "Female",
                            Name = "Tani",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("2a6d3738-9def-43ac-9279-0310edc7ceca"),
                            Address = "97570 Raven Circle",
                            CountryID = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            DateOfBirth = new DateTime(1988, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mlingfoot5@netvibes.com",
                            Gender = "Male",
                            Name = "Mitchael",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("29339209-63f5-492f-8459-754943c74abf"),
                            Address = "57449 Brown Way",
                            CountryID = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            DateOfBirth = new DateTime(1983, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mjarrell6@wisc.edu",
                            Gender = "Male",
                            Name = "Maddy",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("ac660a73-b0b7-4340-abc1-a914257a6189"),
                            Address = "4 Stuart Drive",
                            CountryID = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            DateOfBirth = new DateTime(1998, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "pretchford7@virginia.edu",
                            Gender = "Female",
                            Name = "Pegeen",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("012107df-862f-4f16-ba94-e5c16886f005"),
                            Address = "413 Sachtjen Way",
                            CountryID = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            DateOfBirth = new DateTime(1990, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "hmosco8@tripod.com",
                            Gender = "Male",
                            Name = "Hansiain",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("cb035f22-e7cf-4907-bd07-91cfee5240f3"),
                            Address = "484 Clarendon Court",
                            CountryID = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            DateOfBirth = new DateTime(1997, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "lwoodwing9@wix.com",
                            Gender = "Male",
                            Name = "Lombard",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("28d11936-9466-4a4b-b9c5-2f0a8e0cbde9"),
                            Address = "2 Warrior Avenue",
                            CountryID = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            DateOfBirth = new DateTime(1990, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mconachya@va.gov",
                            Gender = "Female",
                            Name = "Minta",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            ID = new Guid("a3b9833b-8a4d-43e9-8690-61e08df81a9a"),
                            Address = "9334 Fremont Street",
                            CountryID = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            DateOfBirth = new DateTime(1987, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "vklussb@nationalgeographic.com",
                            Gender = "Female",
                            Name = "Verene",
                            ReceiveNewsLetters = false
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Entities.Person", b =>
                {
                    b.HasOne("Entities.Country", "Country")
                        .WithMany("Persons")
                        .HasForeignKey("CountryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Entities.IdentityEntities.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Entities.IdentityEntities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Entities.IdentityEntities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Entities.IdentityEntities.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.IdentityEntities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Entities.IdentityEntities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entities.Country", b =>
                {
                    b.Navigation("Persons");
                });
#pragma warning restore 612, 618
        }
    }
}
