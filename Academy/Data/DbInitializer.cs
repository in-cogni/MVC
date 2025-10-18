using Academy.Models;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data
{
	public class DbInitializer
	{
		public static void Initialize(UniversityContext context)
		{
			try
			{
				context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ""Directions"" (
                ""DirectionId"" SMALLSERIAL PRIMARY KEY,
                ""Name"" VARCHAR(50) NOT NULL
            );
        ");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Note: Directions table may already exist: {ex.Message}");
			}

			try
			{
				context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ""Groups"" (
                ""GroupId"" SERIAL PRIMARY KEY,
                ""GroupName"" VARCHAR(10) NOT NULL,
                ""DirectionId"" SMALLINT NULL,
                ""WeekDays"" SMALLINT NULL,
                ""StartTime"" TIME NULL,
                CONSTRAINT ""FK_Groups_Directions_DirectionId"" FOREIGN KEY (""DirectionId"") REFERENCES ""Directions"" (""DirectionId"") ON DELETE SET NULL
            );
        ");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Note: Groups table may already exist: {ex.Message}");
			}

			try
			{
				context.Database.ExecuteSqlRaw(@"
            DO $$ 
            BEGIN 
                IF NOT EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE table_name = 'Students' AND column_name = 'PhotoPath'
                ) THEN
                    ALTER TABLE ""Students"" ADD COLUMN ""PhotoPath"" TEXT;
                END IF;
            END $$;
        ");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding PhotoPath column: {ex.Message}");
			}

			context.Database.EnsureCreated();

			context.Database.ExecuteSqlRaw("DELETE FROM \"CourseAssignments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Enrollments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"OfficeAssignments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Groups\""); 
			context.Database.ExecuteSqlRaw("DELETE FROM \"Directions\""); 
			context.Database.ExecuteSqlRaw("DELETE FROM \"Courses\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Departments\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Instructors\"");
			context.Database.ExecuteSqlRaw("DELETE FROM \"Students\"");

			var instructors = new Instructor[]
			{
				new Instructor { FirstName = "Олег", LastName = "Ковтун",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2009-04-04"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Марина", LastName = "Покидюк",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2022-07-07"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Андрей", LastName = "Кобылинский",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2003-09-01"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Алексей", LastName = "Свищев",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2013-09-01"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Александр", LastName = "Лялька",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2008-09-25"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Елена", LastName = "Хлапонина",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2008-10-25"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Антон", LastName = "Глазунов",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2022-02-01"), DateTimeKind.Utc) },
				new Instructor { FirstName = "Александр", LastName = "Твердохлеб",
					HireDate = DateTime.SpecifyKind(DateTime.Parse("2007-10-10"), DateTimeKind.Utc) }
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
		InstructorID = instructors.Single(i => i.LastName == "Ковтун").ID },
    new Department { Name = "Mathematics", Budget = 100000,
		StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		InstructorID = instructors.Single(i => i.LastName == "Покидюк").ID },
    new Department { Name = "Engineering", Budget = 350000,
		StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		InstructorID = instructors.Single(i => i.LastName == "Кобылинский").ID },
    new Department { Name = "Economics", Budget = 100000,
		StartDate = DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		InstructorID = instructors.Single(i => i.LastName == "Свищев").ID } 
};

			foreach (Department d in departments)
			{
				context.Departments.Add(d);
			}
			context.SaveChanges();

			var directions = new Direction[]
			{
				new Direction { DirectionId = 1, Name = "Разработка программного обеспечения" },
				new Direction { DirectionId = 2, Name = "Сетевые технологии и системное администрирование" },
				new Direction { DirectionId = 3, Name = "Компьютерная графика и дизайн" },
				new Direction { DirectionId = 4, Name = "Java development" },
				new Direction { DirectionId = 5, Name = "C++ Development" },
				new Direction { DirectionId = 6, Name = "Python development" },
				new Direction { DirectionId = 7, Name = "GameDev" },
				new Direction { DirectionId = 8, Name = "Android development" },
				new Direction { DirectionId = 9, Name = ".NET Development" },
				new Direction { DirectionId = 10, Name = "WebDev" },
				new Direction { DirectionId = 11, Name = "AI" },

			};

			foreach (Direction d in directions)
			{
				context.Directions.Add(d);
			}
			context.SaveChanges();

			var students = new Student[]
{
	new Student{
		FirstName="Никита", LastName="Сивков",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2005-09-01"), DateTimeKind.Utc),
		PhotoPath="student1.jpg"
	},
	new Student{
		FirstName="Мария", LastName="Тарлавина",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2002-09-01"), DateTimeKind.Utc),
		PhotoPath="student2.jpg"
	},
	new Student{
		FirstName="Александр", LastName="Ким",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2003-09-01"), DateTimeKind.Utc),
		PhotoPath="student3.jpg"
	},
	new Student{
		FirstName="Кирилл", LastName="Колпаков",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		PhotoPath="student4.jpg"
	},
	new Student{
		FirstName="Евгений", LastName="Долженков",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2004-09-01"), DateTimeKind.Utc),
		PhotoPath="student5.jpg"
	},
	new Student{
		FirstName="Павел", LastName="Третьяков",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2000-09-01"), DateTimeKind.Utc),
		PhotoPath="student6.jpg"
	},
	new Student{
		FirstName="Михаил", LastName="Шеркунов",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1996-09-01"), DateTimeKind.Utc),
		PhotoPath="student7.jpg"
	},
	new Student{
		FirstName="Александр", LastName="Архипов",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1991-09-01"), DateTimeKind.Utc),
		PhotoPath="student8.jpg"
	},
	new Student{
		FirstName="Сергей", LastName="Епягин",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1996-09-01"), DateTimeKind.Utc),
		PhotoPath="student1.jpg"
	},
	new Student{
		FirstName="Иван", LastName="Путинцев",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1990-09-01"), DateTimeKind.Utc),
		PhotoPath="student2.jpg"
	},
	new Student{
		FirstName="Роман", LastName="Сутугин",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1996-09-01"), DateTimeKind.Utc),
		PhotoPath="student3.jpg"
	},
	new Student{
		FirstName="Артем", LastName="Фурсов",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1999-09-01"), DateTimeKind.Utc),
		PhotoPath="student4.jpg"
	},
	new Student{
		FirstName="Кирсан", LastName="Терешкин",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		PhotoPath="student5.jpg"
	},
	new Student{
		FirstName="Артем", LastName="Васильев",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1999-09-01"), DateTimeKind.Utc),
		PhotoPath="student6.jpg"
	},
	new Student{
		FirstName="Евгений", LastName="Тихомиров",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1986-09-01"), DateTimeKind.Utc),
		PhotoPath="student7.jpg"
	},
	new Student{
		FirstName="Илья", LastName="Шадько",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		PhotoPath="student8.jpg"
	},
	new Student{
		FirstName="Андрей", LastName="Шмидт",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2007-09-01"), DateTimeKind.Utc),
		PhotoPath="student1.jpg"
	},
	new Student{
		FirstName="Ярослав", LastName="Шуников",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2001-09-01"), DateTimeKind.Utc),
		PhotoPath="student2.jpg"
	},
	new Student{
		FirstName="Олеся", LastName="Волкова",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1985-09-01"), DateTimeKind.Utc),
		PhotoPath="student3.jpg"
	},
	new Student{
		FirstName="Арамис", LastName="Григорян",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1995-09-01"), DateTimeKind.Utc),
		PhotoPath="student4.jpg"
	},
	new Student{
		FirstName="Сергей", LastName="Клочко",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1984-09-01"), DateTimeKind.Utc),
		PhotoPath="student5.jpg"
	},
	new Student{
		FirstName="Сергей", LastName="Корнюшин",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1981-09-01"), DateTimeKind.Utc),
		PhotoPath="student6.jpg"
	},
	new Student{
		FirstName="Дмитрий", LastName="Моисеев",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("2001-09-01"), DateTimeKind.Utc),
		PhotoPath="student7.jpg"
	},
	new Student{
		FirstName="Андрей", LastName="Пластинин",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1979-09-01"), DateTimeKind.Utc),
		PhotoPath="student8.jpg"
	},
	new Student{
		FirstName="Станислав", LastName="Терещенков",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1985-09-01"), DateTimeKind.Utc),
		PhotoPath="student1.jpg"
	},
	new Student{
		FirstName="Алексей", LastName="Хорев",
		EnrollmentDate=DateTime.SpecifyKind(DateTime.Parse("1977-09-01"), DateTimeKind.Utc),
		PhotoPath="student2.jpg"
	}
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

			var groups = new Group[]
			{
				new Group { GroupName = "PU_211", DirectionId = 1, WeekDays = 5, StartTime = TimeSpan.Parse("09:00:00") },
				new Group { GroupName = "PV_211", DirectionId = 1, WeekDays = 3, StartTime = TimeSpan.Parse("10:30:00") },
				new Group { GroupName = "PD_212", DirectionId = 1, WeekDays = 4, StartTime = TimeSpan.Parse("13:15:00") },
				new Group { GroupName = "SU_321", DirectionId = 2, WeekDays = 5, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "DU_123", DirectionId = 3, WeekDays = 5, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "DD_311", DirectionId = 3, WeekDays = 5, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "PD_321", DirectionId = 1, WeekDays = 42, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "Java_326", DirectionId = 4, WeekDays = 10, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "PU_212", DirectionId = 1, WeekDays = 21, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "PV_212", DirectionId = 1, WeekDays = 21, StartTime = TimeSpan.Parse("15:00:00") },
				new Group { GroupName = "PV_319", DirectionId = 1, WeekDays = 42, StartTime = TimeSpan.Parse("18:30:00") },
			};

			foreach (Group g in groups)
			{
				context.Groups.Add(g);
			}
			context.SaveChanges();

			var savedStudents = context.Students.ToList();

			var enrollments = new Enrollment[]
			{
				new Enrollment{StudentID=savedStudents[0].ID, CourseID=1050, Grade=Grade.A},
				new Enrollment{StudentID=savedStudents[0].ID, CourseID=4022, Grade=Grade.C},
				new Enrollment{StudentID=savedStudents[0].ID, CourseID=4041, Grade=Grade.B},
				new Enrollment{StudentID=savedStudents[1].ID, CourseID=1045, Grade=Grade.B},
				new Enrollment{StudentID=savedStudents[1].ID, CourseID=3141, Grade=Grade.F},
				new Enrollment{StudentID=savedStudents[1].ID, CourseID=2021, Grade=Grade.F},
				new Enrollment{StudentID=savedStudents[2].ID, CourseID=1050},
				new Enrollment{StudentID=savedStudents[3].ID, CourseID=1050},
				new Enrollment{StudentID=savedStudents[3].ID, CourseID=4022, Grade=Grade.F},
				new Enrollment{StudentID=savedStudents[4].ID, CourseID=4041, Grade=Grade.C},
				new Enrollment{StudentID=savedStudents[5].ID, CourseID=1045},
				new Enrollment{StudentID=savedStudents[6].ID, CourseID=3141, Grade=Grade.A},
			};

			foreach (Enrollment e in enrollments)
			{
				context.Enrollments.Add(e);
			}
			context.SaveChanges();

			var officeAssignments = new OfficeAssignment[]
{
	new OfficeAssignment {
		InstructorID = instructors.Single(i => i.LastName == "Покидюк").ID,
        Location = "Smith 17" },
	new OfficeAssignment {
		InstructorID = instructors.Single(i => i.LastName == "Кобылинский").ID, 
        Location = "Gowan 27" },
	new OfficeAssignment {
		InstructorID = instructors.Single(i => i.LastName == "Свищев").ID, 
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
		InstructorID = instructors.Single(i => i.LastName == "Свищев").ID
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Кобылинский").ID 
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Лялька").ID 
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Macroeconomics").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Лялька").ID
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Calculus").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Покидюк").ID 
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Trigonometry").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Кобылинский").ID
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Composition").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Ковтун").ID 
    },
	new CourseAssignment {
		CourseID = courses.Single(c => c.Title == "Literature").CourseID,
		InstructorID = instructors.Single(i => i.LastName == "Ковтун").ID 
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