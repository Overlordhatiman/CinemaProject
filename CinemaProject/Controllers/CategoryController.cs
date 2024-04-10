using System.Linq;
using CinemaProject.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CinemaContext _context;

        public CategoryController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Category
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            // Calculate quantity of movies and nesting level for each category
            foreach (var category in categories)
            {
                category.MovieCount = _context.FilmCategories.Count(fc => fc.CategoryId == category.Id);
                category.NestingLevel = CalculateNestingLevel(category);
            }

            return View(categories);
        }


        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.ParentCategories = _context.Categories.ToList();
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (IsParentCategoryValid(category.Id, category.ParentCategoryId))
                {
                    _context.Add(category);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ParentCategoryId", "Invalid parent category selection.");
                }
            }

            ViewBag.ParentCategories = _context.Categories.ToList();
            return View(category);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,ParentCategoryId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Category/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var categoryToDelete = _context.Categories.Include(c => c.ChildCategories).FirstOrDefault(c => c.Id == id);
            if (categoryToDelete == null)
            {
                return NotFound();
            }

            var parentId = categoryToDelete.ParentCategoryId;

            // Retrieve all child categories
            var childCategories = categoryToDelete.ChildCategories.ToList();

            // Update parent category ID of each child category
            foreach (var childCategory in childCategories)
            {
                childCategory.ParentCategoryId = parentId;
            }

            // Delete the category
            _context.Categories.Remove(categoryToDelete);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        // Method to check if the selected parent category is not the category itself or one of its descendants
        private bool IsParentCategoryValid(int categoryId, int? parentCategoryId)
        {
            if (parentCategoryId == null)
            {
                return true; // No parent category selected, so it's valid
            }

            // Fetch the parent category
            var parentCategory = _context.Categories.Find(parentCategoryId);

            // Check if the parent category is not the category itself
            if (parentCategoryId == categoryId)
            {
                return false;
            }

            // Check if the parent category is not one of its descendants
            var ancestors = GetAncestors(categoryId);
            if (ancestors.Contains(parentCategoryId.Value))
            {
                return false;
            }

            return true;
        }

        // Method to get all ancestor categories of a category
        private List<int> GetAncestors(int categoryId)
        {
            var ancestors = new List<int>();

            var category = _context.Categories.Find(categoryId);
            while (category != null && category.ParentCategoryId != null)
            {
                ancestors.Add(category.ParentCategoryId.Value);
                category = _context.Categories.Find(category.ParentCategoryId);
            }

            return ancestors;
        }

        // Recursive function to calculate the nesting level of a category
        private int CalculateNestingLevel(Category category)
        {
            int nestingLevel = 0;
            var parentCategory = category.ParentCategory;
            while (parentCategory != null)
            {
                nestingLevel++;
                parentCategory = parentCategory.ParentCategory;
            }
            return nestingLevel;
        }
    }
}
