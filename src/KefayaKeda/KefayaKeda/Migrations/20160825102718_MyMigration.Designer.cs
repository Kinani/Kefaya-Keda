using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using KefayaKeda.Models;

namespace KefayaKeda.Migrations
{
    [DbContext(typeof(KKProfileContext))]
    [Migration("20160825102718_MyMigration")]
    partial class MyMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("KefayaKeda.Models.KKProfile", b =>
                {
                    b.Property<int>("KKProfileId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("LastEdit");

                    b.Property<DateTime>("SessionStartTime");

                    b.Property<TimeSpan>("TimeAllowance");

                    b.Property<TimeSpan>("TimeEllapsed");

                    b.Property<string>("WhatAction");

                    b.Property<string>("name");

                    b.HasKey("KKProfileId");

                    b.ToTable("KKProfiles");
                });
        }
    }
}
