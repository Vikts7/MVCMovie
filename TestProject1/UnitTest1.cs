using NUnit.Framework;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Controllers;
using MvcMovie.Data;

using MvcMovie.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace TestProject1
{
    [TestFixture]
    // This is a test class for the MoviesController
    public class Tests
    {


        [SetUp]
        public void Setup()
        {
        }
        // This test checks if the Movie model is created correctly
        [Test]
        public void TestProductModel()
        {
            var movie = new Movie
            {
                Id = 1,
                Title = "Test Movie",
                ReleaseDate = DateTime.Now,
                Genre = "Action",
                Price = 9.99M
            };
            Assert.That(movie.Id, Is.EqualTo(1));
            Assert.That(movie.Title, Is.EqualTo("Test Movie"));
            Assert.That(movie.Genre, Is.EqualTo("Action"));
        }
        // This test checks if the Movie model is created correctly
        [Test]
        public async Task Edit_ValidId_ShouldUpdateMovie()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
              .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Seed the database with an initial movie
            using (var context = new MvcMovieContext(options))
            {
                context.Movie.Add(new Movie
                {
                    Id = 1,
                    Title = "Old Movie",
                    ReleaseDate = DateTime.Now.AddYears(-1),
                    Genre = "Drama",
                    Price = 10.99m,
                    Country = "USA"
                });
                context.SaveChanges();
            }

            // Perform the edit operation
            using (var context = new MvcMovieContext(options))
            {
                var controller = new MoviesController(context);
                var updatedMovie = new Movie
                {
                    Id = 1,
                    Title = "Updated Movie",
                    ReleaseDate = DateTime.Now,
                    Genre = "Action",
                    Price = 20.99m,
                    Country = "USA"
                };

                var result = await controller.Edit(1, updatedMovie) as RedirectToActionResult;

                // Assert the result
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ActionName, Is.EqualTo("Index"));

                // Verify the movie was updated
                var movieInDb = context.Movie.Find(1);
                Assert.That(movieInDb, Is.Not.Null);
                Assert.That(movieInDb.Title, Is.EqualTo("Updated Movie"));
                Assert.That(movieInDb.Genre, Is.EqualTo("Action"));
                Assert.That(movieInDb.Price, Is.EqualTo(20.99m));
            }
        }
        // This test checks if the Edit action returns the correct view when an invalid ID is provided
        [Test]
        public async Task Delete_ValidId_ShouldUpdateMovie()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;
            using (var context = new MvcMovieContext(options))
            {
                context.Movie.Add(new Movie { Id = 1, Title = "Test Movie", ReleaseDate = DateTime.Now, Genre = "Action", Price = 9.99m, Country="Russia" });
                context.SaveChanges();
            
            }
            using (var context=new MvcMovieContext(options))
            {
                var controller = new MoviesController(context);
                var result=await controller.DeleteConfirmed(1) as RedirectToActionResult;
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                var deletedMovie = context.Movie.Find(1);
                Assert.That(deletedMovie, Is.Null);
            }
        }
  
        [Test]
        // Test for Details action
        // This test checks if the Details action returns the correct movie when a valid ID is provided
        public async Task Details_ValidId_ShouldReturnProduct()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using(var context = new MvcMovieContext(options))
            {
                context.Movie.Add(new Movie {
                    Id = 1,
                    Title = "Test Movie",
                    ReleaseDate = DateTime.Now,
                    Genre = "Action",
                    Price = 9.99m,
                    Country = "USA"
                });
                context.SaveChanges();
            }
            using(var context = new MvcMovieContext(options))
            {
                var controller = new MoviesController(context);
                var result = await controller.Details(1) as ViewResult;
                var movie = result.Model as Movie;
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<ViewResult>());
                Assert.That(movie, Is.Not.Null);
                Assert.That(movie.Id, Is.EqualTo(1));
                Assert.That(movie.Title, Is.EqualTo("Test Movie"));
                Assert.That(movie.Country, Is.EqualTo("USA"));
            }
        }
        // This test checks if the Details action returns a NotFound result when an invalid ID is provided
        [Test]
        public async Task Details_InvalidId_ShouldReturnNotFound()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            using(var context=new MvcMovieContext(options))
            {
                var controller=new MoviesController(context);
                var result = await controller.Details(999) as NotFoundResult;
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }
        // This test checks if the Index action returns a view with a list of movies
        [Test]
        public async Task Index_ShouldReturnViewWithMovies()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;
            using(var context=new MvcMovieContext(options))
            {
                context.Movie.Add(new Movie { Id = 1, Title = "Test Movie", ReleaseDate = DateTime.Now, Genre = "Action", Price = 9.99m, Country="Bulgaria" });
                context.Movie.Add(new Movie { Id = 2, Title = "Another Movie", ReleaseDate = DateTime.Now, Genre = "Drama", Price = 12.99m, Country="Italy" });
                context.SaveChanges();
            }
            using(var context=new MvcMovieContext(options))
            {
                var controller=new MoviesController(context);
                var result = await controller.Index() as ViewResult;
                var movies = result.Model as List<Movie>;
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<ViewResult>());
                Assert.That(movies, Is.Not.Null);
                Assert.That(movies.Count, Is.EqualTo(2));
                Assert.That(movies[0].Title, Is.EqualTo("Test Movie"));
                Assert.That(movies[1].Title, Is.EqualTo("Another Movie"));
                Assert.That(movies[0].Country, Is.EqualTo("Bulgaria"));
            }
        }
        [Test]
        // This test checks if the Create action returns the correct view when a valid movie is created
        public async Task Create_ValidMovie_ShouldRedirectToIndex()
        {
            var options = new DbContextOptionsBuilder<MvcMovieContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using(var context=new MvcMovieContext(options))
            {
                var controller=new MoviesController(context);
                var movie=new Movie
                {
                    Id = 1,
                    Title = "New Movie",
                    ReleaseDate = DateTime.Now,
                    Genre = "Action",
                    Price = 9.99m,
                    Country = "USA"
                };
                var result = await controller.Create(movie) as RedirectToActionResult;
           Assert.That(result, Is.Not.Null);
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                var createdMovie = context.Movie.Find(1);
                Assert.That(createdMovie, Is.Not.Null);
                Assert.That(createdMovie.Title, Is.EqualTo("New Movie"));
                Assert.That(createdMovie.Genre, Is.EqualTo("Action"));
                Assert.That(createdMovie.Price, Is.EqualTo(9.99m));
                Assert.That(createdMovie.Country, Is.EqualTo("USA"));
            }
        }
    }
}
