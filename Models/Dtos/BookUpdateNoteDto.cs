using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitapTakipMauii.Models.Dtos
{
	public class BookUpdateNoteDto
	{
		public int Id { get; set; }

		[StringLength(500, ErrorMessage = "Not 500 karakterden uzun olamaz.")]
		public string? Notes { get; set; }
	}
}
