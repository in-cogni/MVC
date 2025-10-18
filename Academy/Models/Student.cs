using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Academy.Models
{
	public class Student
	{
		public int ID { get; set; }

		[Required]
		[DisplayName("Фамилия")]
		[StringLength(24)]
		[RegularExpression(@"^[A-ZА-ЯЁ]+[a-zа-яё]*$")]
		public string LastName { get; set; }

		[Required]
		[Display(Name = "Имя")]
		[StringLength(24)]
		[RegularExpression(@"^[A-ZА-ЯЁ]+[a-zа-яё]*$")]
		public string FirstName { get; set; }

		[Required]
		[DisplayName("Дата поступления")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime EnrollmentDate { get; set; }//Дата поступления

		[DisplayName("Фото")]
		public string PhotoPath { get; set; }

		//Calculated properties:
		[DisplayName("Студент")]
		public string FullName { get => $"{LastName} {FirstName}"; }

		//////////////////////////////////////////////////////////////////

		//Navigation property:
		public ICollection<Enrollment> Enrollments { get; set; }

	}
}
