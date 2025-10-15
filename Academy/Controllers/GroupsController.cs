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
	public class GroupsController : Controller
	{
		private readonly UniversityContext _context;

		public GroupsController(UniversityContext context)
		{
			_context = context;
		}

		// GET: Groups
		public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
		{
			ViewData["CurrentSort"] = sortOrder;
			ViewData["GroupNameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "groupname-desc" : "";
			ViewData["WeekDaysSortParam"] = sortOrder == "weekdays" ? "weekdays-desc" : "weekdays";
			ViewData["StartTimeSortParam"] = sortOrder == "starttime" ? "starttime-desc" : "starttime";
			ViewData["DirectionSortParam"] = sortOrder == "direction" ? "direction-desc" : "direction";

			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			ViewData["CurrentFilter"] = searchString;

			IQueryable<Group> groups = _context.Groups.Include(g => g.Direction);

			if (!String.IsNullOrEmpty(searchString))
			{
				groups = groups.Where(g => g.GroupName.Contains(searchString) ||
										 g.Direction.Name.Contains(searchString));
			}

			switch (sortOrder)
			{
				case "groupname-desc":
					groups = groups.OrderByDescending(g => g.GroupName);
					break;
				case "weekdays":
					groups = groups.OrderBy(g => g.WeekDays);
					break;
				case "weekdays-desc":
					groups = groups.OrderByDescending(g => g.WeekDays);
					break;
				case "starttime":
					groups = groups.OrderBy(g => g.StartTime);
					break;
				case "starttime-desc":
					groups = groups.OrderByDescending(g => g.StartTime);
					break;
				case "direction":
					groups = groups.OrderBy(g => g.Direction.Name);
					break;
				case "direction-desc":
					groups = groups.OrderByDescending(g => g.Direction.Name);
					break;
				default:
					groups = groups.OrderBy(g => g.GroupName);
					break;
			}

			int pageSize = 3;
			return View(await PaginatedList<Group>.CreateAsync(groups.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		// GET: Groups/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var group = await _context.Groups
				.Include(g => g.Direction)
				.FirstOrDefaultAsync(m => m.GroupId == id);
			if (group == null)
			{
				return NotFound();
			}

			return View(group);
		}

		// GET: Groups/Create
		public IActionResult Create()
		{
			ViewData["DirectionId"] = new SelectList(_context.Directions, "DirectionId", "Name");
			return View();
		}

		// POST: Groups/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("GroupId,GroupName,DirectionId,WeekDays,StartTime")] Group group)
		{
			if (ModelState.IsValid)
			{
				_context.Add(group);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["DirectionId"] = new SelectList(_context.Directions, "DirectionId", "Name", group.DirectionId);
			return View(group);
		}

		// GET: Groups/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var group = await _context.Groups.FindAsync(id);
			if (group == null)
			{
				return NotFound();
			}
			ViewData["DirectionId"] = new SelectList(_context.Directions, "DirectionId", "Name", group.DirectionId);
			return View(group);
		}

		// POST: Groups/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("GroupId,GroupName,DirectionId,WeekDays,StartTime")] Group group)
		{
			if (id != group.GroupId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(group);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!GroupExists(group.GroupId))
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
			ViewData["DirectionId"] = new SelectList(_context.Directions, "DirectionId", "Name", group.DirectionId);
			return View(group);
		}

		// GET: Groups/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var group = await _context.Groups
				.Include(g => g.Direction)
				.FirstOrDefaultAsync(m => m.GroupId == id);
			if (group == null)
			{
				return NotFound();
			}

			return View(group);
		}

		// POST: Groups/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var group = await _context.Groups.FindAsync(id);
			if (group != null)
			{
				_context.Groups.Remove(group);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool GroupExists(int id)
		{
			return _context.Groups.Any(e => e.GroupId == id);
		}
	}
}