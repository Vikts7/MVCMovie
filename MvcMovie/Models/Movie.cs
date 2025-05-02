using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    /// This class represents a movie entity in the application.
    public class Movie
    {
        /// This property is the primary key for the Movie entity.
        public int Id { get; set; }
        /// This property represents the title of the movie.
        public string Title { get; set; }
        /// This property represents the release date of the movie.
        [DataType(DataType.Date)]
        // This property is used to display the date in a specific format.
        public DateTime ReleaseDate { get; set; }
        /// This property represents the genre of the movie.
        public string? Genre { get; set; }
        /// This property represents the price of the movie.
        public decimal Price { get; set; }
        /// This property represents the county of the movie.
       [Required]
        public string Country { get; set; }
    }
}
