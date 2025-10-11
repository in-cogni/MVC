using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversityHW.Models
{
	public enum Grade {A,B,C,D,F}
	public class Enrollment
	{
		public int EnrollmentID { get; set; }
		public int CourseID { get; set; }
		public int StudentName {  get; set; }
		public Grade? Grade { get; set; }
		
		//Navigation properties:
		public Course Course { get; set; }
		public Student Student { get; set; }
	}
}
