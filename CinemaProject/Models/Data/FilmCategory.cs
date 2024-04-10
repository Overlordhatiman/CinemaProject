using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaProject.Models.Data
{
    public class FilmCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FilmId { get; set; }

        public int CategoryId { get; set; }

        // Navigation properties
        public Film Film { get; set; }
        public Category Category { get; set; }
    }
}
