using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
// This class represents the MoviesController for the MvcMovie application.
{
    public class MoviesController : Controller
    {
        // This is the database context for the MvcMovie application.
        private readonly MvcMovieContext _context;
        // This constructor is used to initialize the MoviesController with a database context.
        public MoviesController(MvcMovieContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> CleanAndReset()
        {
            // 1. Delete all records
            var allMovies = _context.Movie.ToList();
            _context.Movie.RemoveRange(allMovies);
            await _context.SaveChangesAsync();

            // 2. Reset ID counter
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Movie', RESEED, 0)");

            return RedirectToAction(nameof(Index));
        }
        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewBag.Countries = GetCountriesList();
            return View();
        }
        private List<SelectListItem> GetCountriesList()
        {
            return new List<SelectListItem>
            {
               new SelectListItem { Value = "USA", Text = "🇺🇸 USA" },
        new SelectListItem { Value = "UK", Text = "🇬🇧 UK" },
        new SelectListItem { Value = "Germany", Text = "🇩🇪 Germany" },
        new SelectListItem { Value = "France", Text = "🇫🇷 France" },
        new SelectListItem { Value = "Japan", Text = "🇯🇵 Japan" },
        new SelectListItem{Value="Russia", Text="🇷🇺 Russia"},
        new SelectListItem{Value="Bulgaria", Text="🇧🇬 Bulgaria"}
            };
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Country")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewBag.Countries = GetCountriesList();
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Country")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
