﻿// <auto-generated />
using System;
using ITMO.Dev.ASAP.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ITMO.Dev.ASAP.DataAccess.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230428120329_RemovedSubjectCourseGoogleAssociation")]
    partial class RemovedSubjectCourseGoogleAssociation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.DeadlinePolicies.DeadlinePolicy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SpanBeforeActivation")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCourseId");

                    b.ToTable("DeadlinePolicy", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("DeadlinePolicy");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.Assignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("MaxPoints")
                        .HasColumnType("double precision");

                    b.Property<double>("MinPoints")
                        .HasColumnType("double precision");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCourseId");

                    b.HasIndex("ShortName", "SubjectCourseId")
                        .IsUnique();

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.GroupAssignment", b =>
                {
                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Deadline")
                        .HasColumnType("date");

                    b.HasKey("GroupId", "AssignmentId");

                    b.HasIndex("AssignmentId");

                    b.ToTable("GroupAssignments");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.StudentGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("StudentGroups");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.Subject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("WorkflowType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("SubjectCourses");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.SubjectCourseGroup", b =>
                {
                    b.Property<Guid>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentGroupId")
                        .HasColumnType("uuid");

                    b.HasKey("SubjectCourseId", "StudentGroupId");

                    b.HasIndex("StudentGroupId");

                    b.ToTable("SubjectCourseGroups");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Submissions.Submission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Code")
                        .HasColumnType("integer");

                    b.Property<double?>("ExtraPoints")
                        .HasColumnType("double precision");

                    b.Property<Guid>("GroupAssignmentAssignmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupAssignmentGroupId")
                        .HasColumnType("uuid");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("Rating")
                        .HasColumnType("double precision");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("StudentUserId");

                    b.HasIndex("GroupAssignmentGroupId", "GroupAssignmentAssignmentId");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.UserAssociations.UserAssociation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "Discriminator")
                        .IsUnique();

                    b.ToTable("UserAssociations");

                    b.HasDiscriminator<string>("Discriminator").HasValue("UserAssociation");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Users.Mentor", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("Mentors");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Users.Student", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId");

                    b.HasIndex("GroupId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.DeadlinePolicies.AbsoluteDeadlinePolicy", b =>
                {
                    b.HasBaseType("ITMO.Dev.ASAP.Domain.DeadlinePolicies.DeadlinePolicy");

                    b.Property<double>("AbsoluteValue")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue("AbsoluteDeadlinePolicy");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.DeadlinePolicies.CappingDeadlinePolicy", b =>
                {
                    b.HasBaseType("ITMO.Dev.ASAP.Domain.DeadlinePolicies.DeadlinePolicy");

                    b.Property<double>("Cap")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue("CappingDeadlinePolicy");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.DeadlinePolicies.FractionDeadlinePolicy", b =>
                {
                    b.HasBaseType("ITMO.Dev.ASAP.Domain.DeadlinePolicies.DeadlinePolicy");

                    b.Property<double>("Fraction")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue("FractionDeadlinePolicy");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.UserAssociations.IsuUserAssociation", b =>
                {
                    b.HasBaseType("ITMO.Dev.ASAP.Domain.UserAssociations.UserAssociation");

                    b.Property<int>("UniversityId")
                        .HasColumnType("integer");

                    b.HasIndex("UniversityId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("IsuUserAssociation");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.DeadlinePolicies.DeadlinePolicy", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", null)
                        .WithMany("DeadlinePolicies")
                        .HasForeignKey("SubjectCourseId");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.Assignment", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", "SubjectCourse")
                        .WithMany("Assignments")
                        .HasForeignKey("SubjectCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SubjectCourse");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.GroupAssignment", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.Assignment", "Assignment")
                        .WithMany("GroupAssignments")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.StudentGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Assignment");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.Subject", "Subject")
                        .WithMany("Courses")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.SubjectCourseGroup", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.StudentGroup", "StudentGroup")
                        .WithMany()
                        .HasForeignKey("StudentGroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", "SubjectCourse")
                        .WithMany("Groups")
                        .HasForeignKey("SubjectCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("StudentGroup");

                    b.Navigation("SubjectCourse");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Submissions.Submission", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Users.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.GroupAssignment", "GroupAssignment")
                        .WithMany("Submissions")
                        .HasForeignKey("GroupAssignmentGroupId", "GroupAssignmentAssignmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("GroupAssignment");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.UserAssociations.UserAssociation", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Users.User", "User")
                        .WithMany("Associations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Users.Mentor", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", "Course")
                        .WithMany("Mentors")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ITMO.Dev.ASAP.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Users.Student", b =>
                {
                    b.HasOne("ITMO.Dev.ASAP.Domain.Study.StudentGroup", "Group")
                        .WithMany("Students")
                        .HasForeignKey("GroupId");

                    b.HasOne("ITMO.Dev.ASAP.Domain.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("ITMO.Dev.ASAP.Domain.Users.Student", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.Assignment", b =>
                {
                    b.Navigation("GroupAssignments");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.GroupAssignment", b =>
                {
                    b.Navigation("Submissions");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.StudentGroup", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.Subject", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Study.SubjectCourse", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("DeadlinePolicies");

                    b.Navigation("Groups");

                    b.Navigation("Mentors");
                });

            modelBuilder.Entity("ITMO.Dev.ASAP.Domain.Users.User", b =>
                {
                    b.Navigation("Associations");
                });
#pragma warning restore 612, 618
        }
    }
}
