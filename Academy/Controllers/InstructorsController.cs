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

namespace Academy.Controllers
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

			PopulateAssignedCourseData(instructor);
			return View(instructor);
		}

		// GET: Instructors/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Instructors/Create
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

			Instructor instructor = await _context.Instructors
				.Include(i => i.OfficeAssignment)
				.Include(i => i.CourseAssignments)
					.ThenInclude(i => i.Course)
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
			DbSet<Course> allCourses = _context.Courses;
			HashSet<int> instructorCourses = new HashSet<int>
				(instructor.CourseAssignments.Select(c => c.CourseID));
			List<AssignedCourseData> viewModel = new List<AssignedCourseData>();
			foreach (Course course in allCourses)
			{
				viewModel.Add(new AssignedCourseData
				{
					CourseID = course.CourseID,
					Title = course.Title,
					Assigned = instructorCourses.Contains(course.CourseID)
				});
			}
			ViewBag.Courses = viewModel;
		}

		// POST: Instructors/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id,
			[Bind("ID,LastName,FirstName,HireDate")] Instructor instructor,
			string officeLocation,
			string[] selectedCourses)
		{
			var instructorToUpdate = await _context.Instructors
				.Include(i => i.OfficeAssignment)
				.Include(i => i.CourseAssignments)
				.FirstOrDefaultAsync(m => m.ID == id);

			if (instructorToUpdate == null)
			{
				return NotFound();
			}

			instructorToUpdate.LastName = instructor.LastName;
			instructorToUpdate.FirstName = instructor.FirstName;
			instructorToUpdate.HireDate = instructor.HireDate;

			if (string.IsNullOrWhiteSpace(officeLocation))
			{
				if (instructorToUpdate.OfficeAssignment != null)
				{
					_context.OfficeAssignments.Remove(instructorToUpdate.OfficeAssignment);
					instructorToUpdate.OfficeAssignment = null;
				}
			}
			else
			{
				if (instructorToUpdate.OfficeAssignment == null)
				{
					instructorToUpdate.OfficeAssignment = new OfficeAssignment
					{
						InstructorID = instructorToUpdate.ID,
						Location = officeLocation
					};
				}
				else
				{
					instructorToUpdate.OfficeAssignment.Location = officeLocation;
				}
			}

			UpdateInstructorCourses(selectedCourses, instructorToUpdate);

			try
			{
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException)
			{
				ModelState.AddModelError("", "Unable to save changes");
			}

			PopulateAssignedCourseData(instructorToUpdate);
			return View(instructorToUpdate);
		}

		private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
		{
			if (selectedCourses == null)
			{
				foreach (var assignment in instructorToUpdate.CourseAssignments.ToList())
				{
					_context.CourseAssignments.Remove(assignment);
				}
				return;
			}

			var selectedIds = selectedCourses.Select(id => int.Parse(id)).ToHashSet();
			var currentIds = instructorToUpdate.CourseAssignments.Select(ca => ca.CourseID).ToHashSet();

			foreach (var assignment in instructorToUpdate.CourseAssignments.ToList())
			{
				if (!selectedIds.Contains(assignment.CourseID))
				{
					_context.CourseAssignments.Remove(assignment);
				}
			}

			foreach (var courseId in selectedIds)
			{
				if (!currentIds.Contains(courseId))
				{
					instructorToUpdate.CourseAssignments.Add(new CourseAssignment
					{
						InstructorID = instructorToUpdate.ID,
						CourseID = courseId
					});
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