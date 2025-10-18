using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Academy.Data;
using Academy.Models;

namespace Academy.Views
{
	public class StudentsController : Controller
	{
		private readonly UniversityContext _context;
		private readonly IWebHostEnvironment _environment;

		public StudentsController(UniversityContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		// GET: Students
		public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
		{
			ViewData["CurrentSort"] = sortOrder;
			ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name-desc" : "";
			ViewData["DateSortParam"] = sortOrder == "date" ? "date-desc" : "date";

			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			ViewData["CurrentFilter"] = searchString;

			IQueryable<Student> students = from s in _context.Students select s;

			if (!String.IsNullOrEmpty(searchString))
			{
				students = students.Where(s => s.LastName.Contains(searchString) || s.FirstName.Contains(searchString));
			}

			switch (sortOrder)
			{
				case "name-desc": students = students.OrderByDescending(s => s.LastName); break;
				case "date": students = students.OrderBy(s => s.EnrollmentDate); break;
				case "date-desc": students = students.OrderByDescending(s => s.EnrollmentDate); break;
				default: students = students.OrderBy(s => s.LastName); break;
			}

			int pageSize = 3;
			return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		// GET: Students/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.Students
				.FirstOrDefaultAsync(m => m.ID == id);
			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}

		// GET: Students/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Students/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,LastName,FirstName,EnrollmentDate")] Student student, IFormFile photo)
		{
			if (ModelState.IsValid)
			{
				if (photo != null && photo.Length > 0)
				{
					var fileName = await SavePhoto(photo);
					student.PhotoPath = fileName;
				}

				_context.Add(student);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(student);
		}

		// GET: Students/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.Students.FindAsync(id);
			if (student == null)
			{
				return NotFound();
			}
			return View(student);
		}

		// POST: Students/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstName,EnrollmentDate,PhotoPath")] Student student, IFormFile photo)
		{
			if (id != student.ID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (photo != null && photo.Length > 0)
					{
						if (!string.IsNullOrEmpty(student.PhotoPath))
						{
							DeletePhoto(student.PhotoPath);
						}

						var fileName = await SavePhoto(photo);
						student.PhotoPath = fileName;
					}

					_context.Update(student);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!StudentExists(student.ID))
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
			return View(student);
		}

		// GET: Students/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.Students
				.FirstOrDefaultAsync(m => m.ID == id);
			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}

		// POST: Students/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var student = await _context.Students.FindAsync(id);
			if (student != null)
			{
				// Удаляем фото при удалении студента
				if (!string.IsNullOrEmpty(student.PhotoPath))
				{
					DeletePhoto(student.PhotoPath);
				}

				_context.Students.Remove(student);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool StudentExists(int id)
		{
			return _context.Students.Any(e => e.ID == id);
		}

		private async Task<string> SavePhoto(IFormFile photo)
		{
			var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "students");
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}

			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await photo.CopyToAsync(stream);
			}

			return fileName;
		}

		private void DeletePhoto(string photoPath)
		{
			var filePath = Path.Combine(_environment.WebRootPath, "images", "students", photoPath);
			if (System.IO.File.Exists(filePath))
			{
				System.IO.File.Delete(filePath);
			}
		}
	}
}