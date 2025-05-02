using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data
{
    // This class represents the database context for the MvcMovie application.
    public class MvcMovieContext : DbContext
    {
        // This constructor is used to pass options to the base DbContext class.
        // It allows the context to be configured with different options, such as the database provider and connection string.
        public MvcMovieContext (DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }
        // This property represents the Movies table in the database.
        public DbSet<MvcMovie.Models.Movie> Movie { get; set; } = default!;
    }
}
