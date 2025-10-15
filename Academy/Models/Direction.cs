using System.ComponentModel.DataAnnotations;

namespace Academy.Models
{
	public class Direction
	{
		[Key]
		public byte DirectionId { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		//Navigation properties:
		public ICollection<Group> Groups { get; set; }
	}
}
