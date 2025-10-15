using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Academy.Data;
using Academy.Models;
using Academy.Models.ViewModels;

namespace Academy.Views
{
    public class InstructorsController : Controller
    {
        private readonly UniversityContext _context;

        public InstructorsController(UniversityContext context)
        {
            _context = context;
        }

		// GET: Instructors
		public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
		{
			ViewData["CurrentSort"] = sortOrder;
			ViewData["LastNameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "lastname-desc" : "";
			ViewData["FirstNameSortParam"] = sortOrder == "firstname" ? "firstname-desc" : "firstname";
			ViewData["HireDateSortParam"] = sortOrder == "hiredate" ? "hiredate-desc" : "hiredate";

			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			ViewData["CurrentFilter"] = searchString;

			IQueryable<Instructor> instructors = _context.Instructors;

			if (!String.IsNullOrEmpty(searchString))
			{
				instructors = instructors.Where(i => i.LastName.Contains(searchString) ||
												   i.FirstName.Contains(searchString));
			}

			switch (sortOrder)
			{
				case "lastname-desc":
					instructors = instructors.OrderByDescending(i => i.LastName);
					break;
				case "firstname":
					instructors = instructors.OrderBy(i => i.FirstName);
					break;
				case "firstname-desc":
					instructors = instructors.OrderByDescending(i => i.FirstName);
					break;
				case "hiredate":
					instructors = instructors.OrderBy(i => i.HireDate);
					break;
				case "hiredate-desc":
					instructors = instructors.OrderByDescending(i => i.HireDate);
					break;
				default:
					instructors = instructors.OrderBy(i => i.LastName);
					break;
			}

			int pageSize = 3;
			return View(await PaginatedList<Instructor>.CreateAsync(instructors.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		// GET: Instructors/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var instructor = await _context.Instructors
				.Include(i => i.OfficeAssignment)
				.Include(i => i.CourseAssignments)
					.ThenInclude(i => i.Course)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.ID == id);

			if (instructor == null)
			{
				return NotFound();
			}

			// Заполняем данные о курсах для отображения
			PopulateAssignedCourseData(instructor);
			return View(instructor);
		}

		// GET: Instructors/Create
		public IActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstName,HireDate")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var instructor = await _context.Instructors
		.Include(i => i.OfficeAssignment)
		.Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
		.AsNoTracking()
		.FirstOrDefaultAsync(m => m.ID == id);

			if (instructor == null)
            {
                return NotFound();
            }
			PopulateAssignedCourseData(instructor);
			return View(instructor);
        }

		private void PopulateAssignedCourseData(Instructor instructor)
		{
			var allCourses = _context.Courses;
			var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));
			var viewModel = new List<AssignedCourseData>();
			foreach (var course in allCourses)
			{
				viewModel.Add(new AssignedCourseData
				{
					CourseID = course.CourseID,
					Title = course.Title,
					Assigned = instructorCourses.Contains(course.CourseID)
				});
			}
			ViewData["Courses"] = viewModel;
		}

		// POST: Instructors/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
		{
			if (id == null)
			{
				return NotFound();
			}

			var instructorToUpdate = await _context.Instructors
				.Include(i => i.OfficeAssignment)
				.Include(i => i.CourseAssignments)
					.ThenInclude(i => i.Course)
				.FirstOrDefaultAsync(m => m.ID == id);

			if (await TryUpdateModelAsync<Instructor>(
				instructorToUpdate,
				"",
				i => i.FirstName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
			{
				if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
				{
					instructorToUpdate.OfficeAssignment = null;
				}
				UpdateInstructorCourses(selectedCourses, instructorToUpdate);
				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateException /* ex */)
				{
					//Log the error (uncomment ex variable name and write a log.)
					ModelState.AddModelError("", "Unable to save changes. " +
						"Try again, and if the problem persists, " +
						"see your system administrator.");
				}
				return RedirectToAction(nameof(Index));
			}
			UpdateInstructorCourses(selectedCourses, instructorToUpdate);
			PopulateAssignedCourseData(instructorToUpdate);
			return View(instructorToUpdate);
		}

		private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
		{
			if (selectedCourses == null)
			{
				instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
				return;
			}

			var selectedCoursesHS = new HashSet<string>(selectedCourses);
			var instructorCourses = new HashSet<int>
				(instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
			foreach (var course in _context.Courses)
			{
				if (selectedCoursesHS.Contains(course.CourseID.ToString()))
				{
					if (!instructorCourses.Contains(course.CourseID))
					{
						instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.CourseID });
					}
				}
				else
				{

					if (instructorCourses.Contains(course.CourseID))
					{
						CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.FirstOrDefault(i => i.CourseID == course.CourseID);
						_context.Remove(courseToRemove);
					}
				}
			}
		}

		// GET: Instructors/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
