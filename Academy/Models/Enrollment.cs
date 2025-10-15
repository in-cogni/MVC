using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Academy.Models
{
	public enum Grade { A, B, C, D, F }
	public class Enrollment
	{
		public int EnrollmentID { get; set; }
		public int CourseID { get; set; }
		public int StudentID { get; set; }
		[DisplayFormat(NullDisplayText = "Оценка не выставлена")]
		public Grade? Grade { get; set; }

		//Navigation properties:
		public Student Student { get; set; }
		public Course Course { get; set; }
	}
}
