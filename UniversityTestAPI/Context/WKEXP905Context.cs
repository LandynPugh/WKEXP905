﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UniversityTestAPI.Models;

namespace UniversityTestAPI.Context
{
    public partial class WKEXP905Context : DbContext
    {
        public WKEXP905Context()
        {
        }

        public WKEXP905Context(DbContextOptions<WKEXP905Context> options)
            : base(options)
        {
        }

        public virtual DbSet<College> College { get; set; }
        public virtual DbSet<Faculty> Faculty { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<College>(entity =>
            {
                entity.Property(e => e.CollegeId)
                    .HasColumnName("CollegeID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Abbreviation)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("abbreviation");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.Property(e => e.FacultyId)
                    .HasColumnName("FacultyID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Abbreviation)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("abbreviation");

                entity.Property(e => e.CollegeId).HasColumnName("collegeID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FacultyId).HasColumnName("facultyID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("phone");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}