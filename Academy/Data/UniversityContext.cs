using Academy.Models;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data
{
	public class UniversityContext:DbContext
	{
		public UniversityContext(DbContextOptions<UniversityContext> options) : base(options) { }
		public DbSet<Student> Students { get; set; }
		public DbSet<Enrollment> Enrollments { get; set; }
		public DbSet<Course> Courses { get; set; }
		///////////////////////////////////////////////////////////////
		public DbSet<Department> Departments { get; set; }
		public DbSet<Instructor> Instructors { get; set; }
		public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
		public DbSet<CourseAssignment> CourseAssignments { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Student>().ToTable("Students");
			modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
			modelBuilder.Entity<Course>().ToTable("Courses");
			///////////////////////////////////////////////////////////////
			modelBuilder.Entity<Department>().ToTable("Departments");
			modelBuilder.Entity<Instructor>().ToTable("Instructors");
			modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignments");
			modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignments");
			modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.InstructorID });

			modelBuilder.Entity<OfficeAssignment>()
		.HasOne(o => o.Instructor)
		.WithOne(i => i.OfficeAssignment)
		.HasForeignKey<OfficeAssignment>(o => o.InstructorID);

			modelBuilder.Entity<Department>()
				.HasOne(d => d.Administrator)
				.WithMany()
				.HasForeignKey(d => d.InstructorID)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
