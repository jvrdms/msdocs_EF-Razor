using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Courses {
  public class EditModel : DepartmentNamePageModel {
    private readonly SchoolContext _context;

    public EditModel(SchoolContext context) {
      _context = context;
    }

    [BindProperty]
    public Course Course { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id) {
      if (id == null) {
        return NotFound();
      }

      Course = await _context.Courses
          .Include(c => c.Department).FirstOrDefaultAsync(m => m.CourseID == id);

      if (Course == null) {
        return NotFound();
      }
      // select current DepartmentID
      PopulateDepartmentDropDownList(_context, Course.DepartmentID);

      // ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
      return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id) {
      if (!ModelState.IsValid) {
        return Page();
      }

      var courseToUpdate = await _context.Courses.FindAsync(id);

      if (await TryUpdateModelAsync<Course>(
          courseToUpdate,
          "course", // prefix for form value
          c => c.Credits, c => c.DepartmentID, c => c.Title)) {
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
      }

      // populate DepartmentID if TryUpdateModel fails
      PopulateDepartmentDropDownList(_context, courseToUpdate.DepartmentID);
      return Page();

      // _context.Attach(Course).State = EntityState.Modified;

      // try {
      //   await _context.SaveChangesAsync();
      // } catch (DbUpdateConcurrencyException) {
      //   if (!CourseExists(Course.CourseID)) {
      //     return NotFound();
      //   } else {
      //     throw;
      //   }
      // }

      // return RedirectToPage("./Index");
    }

    private bool CourseExists(int id) {
      return _context.Courses.Any(e => e.CourseID == id);
    }
  }
}
