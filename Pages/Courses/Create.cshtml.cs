using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Courses {
  public class CreateModel : DepartmentNamePageModel {
    private readonly SchoolContext _context;

    public CreateModel(SchoolContext context) {
      _context = context;
    }

    public IActionResult OnGet() {
      // ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
      PopulateDepartmentDropDownList(_context);
      return Page();
    }

    [BindProperty]
    public Course Course { get; set; }

    public async Task<IActionResult> OnPostAsync() {
      if (!ModelState.IsValid) {
        return Page();
      }

      var emptyCourse = new Course();

      if (await TryUpdateModelAsync<Course>(
            emptyCourse,
            "course", // prefix fof form value
            s => s.CourseID, s => s.DepartmentID, s => s.Title, s => s.Credits)) {

      _context.Courses.Add(emptyCourse);
      await _context.SaveChangesAsync();
      return RedirectToPage("./Index");
      }

        // select DepartmentID if TryUpdateModelAsync fails
        PopulateDepartmentDropDownList(_context,emptyCourse.DepartmentID);
        return Page();
    }
  }
}