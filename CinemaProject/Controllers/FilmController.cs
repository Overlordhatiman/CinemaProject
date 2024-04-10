using System.Linq;
using CinemaProject.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Controllers
{
    public class FilmController : Controller
    {
        private readonly CinemaContext _context;

        public FilmController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Film
        public IActionResult Index()
        {
            var films = _context.Films
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .ToList();

            var categories = _context.Categories.ToList();

            ViewBag.Categories = categories;

            return View(films);
        }

        // GET: Film/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = _context.Films
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .FirstOrDefault(m => m.Id == id);

            // Get all categories
            var allCategories = _context.Categories.ToList();

            // Get categories already associated with the film
            var categoriesInFilm = film.FilmCategories.Select(fc => fc.Category).ToList();

            // Exclude categories in the film from all categories
            var availableCategories = allCategories.Where(c => !categoriesInFilm.Any(cf => cf.Id == c.Id)).ToList();

            ViewBag.AllCategories = availableCategories;
            ViewBag.CategoriesInFilm = categoriesInFilm;

            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Film/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Film/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Director,Release")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Film/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = _context.Films.Find(id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }

        // POST: Film/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Director,Release")] Film film)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
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
            return View(film);
        }

        // GET: Film/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = _context.Films.FirstOrDefault(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Film/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var film = _context.Films.Find(id);
            _context.Films.Remove(film);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: Film/AddCategoryToFilm/{filmId}?categoryIds={categoryIds}
        [HttpPost]
        public IActionResult AddCategoryToFilm(int filmId, int[] categoryIds)
        {
            var film = _context.Films.Include(f => f.FilmCategories).FirstOrDefault(f => f.Id == filmId);
            if (film == null)
            {
                return NotFound();
            }

            // Remove existing categories from the film
            var categoriesToRemove = film.FilmCategories.Where(fc => categoryIds.Contains(fc.CategoryId)).ToList();
            foreach (var categoryToRemove in categoriesToRemove)
            {
                _context.FilmCategories.Remove(categoryToRemove);
            }

            // Add selected categories to the film
            foreach (var categoryId in categoryIds)
            {
                if (!film.FilmCategories.Any(fc => fc.CategoryId == categoryId))
                {
                    film.FilmCategories.Add(new FilmCategory { CategoryId = categoryId });
                }
            }

            _context.SaveChanges();

            return Ok();
        }


        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
}
