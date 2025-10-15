using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel;
using Academy.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Models
{
	public class Group
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int GroupId { get; set; }

		[Required]
		[StringLength(10)]
		public string GroupName { get; set; }

		public byte? DirectionId { get; set; }

		public byte? WeekDays { get; set; }

		public TimeSpan? StartTime { get; set; }

		//Navigation properties:

		public Direction Direction { get; set; }
	}
}
