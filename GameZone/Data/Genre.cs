using System.ComponentModel.DataAnnotations;
using static GameZone.Constants.ModelConstants.Genre;

namespace GameZone.Data
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public ICollection<Game> Games { get; set; }
            = new HashSet<Game>();
    }
}