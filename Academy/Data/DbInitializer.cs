using Academy.Models;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data
{
	public class DbInitializer
	{
		public static void Initialize(UniversityContext context)
		{
			context.Database.EnsureCreated();
			context.Database.ExecuteSqlRaw("DELETE FROM \"CourseAssignments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Enrollments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"OfficeAssignments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Courses\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Departments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Instructors\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Students\"");

			var instructors = new Instructor[]
			{
				new Instructor { FirstName = "Kim", LastName = "Abercrombie",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("1995-03-11"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Fadi", LastName = "Fakhouri",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2002-07-06"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Roger", LastName = "Harui",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("1998-07-01"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Candace", LastName = "Kapoor",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2001-01-15"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Roger", LastName = "Zheng",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2004-02-12"), DateTimeKind.Utc) }
			};

			foreach (Instructor i in instructors)
			{
				context.Instructors.Add(i);
			}
			context.SaveChanges();

			var departments = new Department[]
			{
				new Department { Name = "English", Budget = 350000,
					StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
					InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID },
				new Department { Name = "Mathematics", Budget = 100000,
					StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
					InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID },
				new Department { Name = "Engineering", Budget = 350000,
					StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
					InstructorID = instructors.Single(i => i.LastName == "Harui").ID },
				new Department { Name = "Economics", Budget = 100000,
					StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
					InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID }
			};

			foreach (Department d in departments)
			{
				context.Departments.Add(d);
			}
			context.SaveChanges();

			var students = new Student[]
			{
				new Student{FirstName="Carson",LastName="Alexander",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2005-09-01"), DateTimeKind.Utc), PhotoPath="student1.jpg"},
				new Student{FirstName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2002-09-01"), DateTimeKind.Utc), PhotoPath="student2.jpg"},
				new Student{FirstName="Arturo",LastName="Anand",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2003-09-01"), DateTimeKind.Utc), PhotoPath="student3.jpg"},
				new Student{FirstName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2002-09-01"), DateTimeKind.Utc), PhotoPath="student4.jpg"},
				new Student{FirstName="Yan",LastName="Li",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2002-09-01"), DateTimeKind.Utc), PhotoPath="student5.jpg"},
				new Student{FirstName="Peggy",LastName="Justice",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2001-09-01"), DateTimeKind.Utc), PhotoPath="student6.jpg"},
				new Student{FirstName="Laura",LastName="Norman",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2003-09-01"), DateTimeKind.Utc), PhotoPath="student7.jpg"},
				new Student{FirstName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2005-09-01"), DateTimeKind.Utc), PhotoPath="student8.jpg"}
			};

			foreach (Student s in students)
			{
				context.Students.Add(s);
			}
			context.SaveChanges();

			var courses = new Course[]
			{
				new Course {CourseID = 1050, Title = "Chemistry", Credits = 3,
					DepartmentID = departments.Single(s => s.Name == "Engineering").DepartmentID
				},
				new Course {CourseID = 4022, Title = "Microeconomics", Credits = 3,
					DepartmentID = departments.Single(s => s.Name == "Economics").DepartmentID
				},
				new Course {CourseID = 4041, Title = "Macroeconomics", Credits = 3,
					DepartmentID = departments.Single(s => s.Name == "Economics").DepartmentID
				},
				new Course {CourseID = 1045, Title = "Calculus", Credits = 4,
					DepartmentID = departments.Single(s => s.Name == "Mathematics").DepartmentID
				},
				new Course {CourseID = 3141, Title = "Trigonometry", Credits = 4,
					DepartmentID = departments.Single(s => s.Name == "Mathematics").DepartmentID
				},
				new Course {CourseID = 2021, Title = "Composition", Credits = 3,
					DepartmentID = departments.Single(s => s.Name == "English").DepartmentID
				},
				new Course {CourseID = 2042, Title = "Literature", Credits = 4,
					DepartmentID = departments.Single(s => s.Name == "English").DepartmentID
				},
			};

			foreach (Course c in courses)
			{
				context.Courses.Add(c);
			}
			context.SaveChanges();

			var enrollments = new Enrollment[]
			{
				new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
				new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
				new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
				new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
				new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
				new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
				new Enrollment{StudentID=3,CourseID=1050},
				new Enrollment{StudentID=4,CourseID=1050},
				new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
				new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
				new Enrollment{StudentID=6,CourseID=1045},
				new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
			};

			foreach (Enrollment e in enrollments)
			{
				context.Enrollments.Add(e);
			}
			context.SaveChanges();

			var officeAssignments = new OfficeAssignment[]
			{
				new OfficeAssignment {
					InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID,
					Location = "Smith 17" },
				new OfficeAssignment {
					InstructorID = instructors.Single(i => i.LastName == "Harui").ID,
					Location = "Gowan 27" },
				new OfficeAssignment {
					InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID,
					Location = "Thompson 304" },
			};

			foreach (OfficeAssignment o in officeAssignments)
			{
				context.OfficeAssignments.Add(o);
			}
			context.SaveChanges();

			var courseInstructors = new CourseAssignment[]
			{
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Harui").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Macroeconomics").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Calculus").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Trigonometry").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Harui").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Composition").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
				},
				new CourseAssignment {
					CourseID = courses.Single(c => c.Title == "Literature").CourseID,
					InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
				},
			};

			foreach (CourseAssignment ci in courseInstructors)
			{
				context.CourseAssignments.Add(ci);
			}
			context.SaveChanges();
		}
	}
}