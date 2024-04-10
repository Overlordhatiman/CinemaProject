using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaProject.Models.Data
{
    public class Film
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [MaxLength(200)]
        public string Name { get; set; }


        [MaxLength(200)]
        public string Director { get; set; }

        public DateTime Release { get; set; }

        // Navigation property for many-to-many relationship with categories
        public ICollection<FilmCategory>? FilmCategories { get; set; }
    }
}
