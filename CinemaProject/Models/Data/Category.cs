using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaProject.Models.Data
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        // Navigation property for parent category
        public Category? ParentCategory { get; set; }

        // Navigation property for child categories
        public ICollection<Category>? ChildCategories { get; set; }

        // Navigation property for many-to-many relationship with films
        public ICollection<FilmCategory>? FilmCategories { get; set; }

        // Additional property to store the quantity of movies in this category
        [NotMapped]
        public int MovieCount { get; set; }

        // Additional property to store the nesting level of this category
        [NotMapped]
        public int NestingLevel { get; set; }
    }
}
