using ContosoUniversityHW.Models;
using ContosoUniversityHW.Models;

namespace ContosoUniversityHW.Models.ViewModels
{
	public class InstructorIndexData
	{
		public IEnumerable<Instructor> Instructors { get; set; }
		public IEnumerable<Course> Courses { get; set; }
		public IEnumerable<Enrollment> Enrollments { get; set; }
	}
}
