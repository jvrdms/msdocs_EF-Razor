using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Pages.Courses {
  public class IndexModel : PageModel {
    private readonly SchoolContext _context;

    public IndexModel(SchoolContext context) {
      _context = context;
    }

    // public IList<Course> Course { get; set; }
    public IList<CourseViewModel> CourseVM { get; set; }
    public async Task OnGetAsync() {
      CourseVM = await _context.Courses
                    .Select(p => new CourseViewModel {
                      CourseID = p.CourseID,
                      Title = p.Title,
                      Credits = p.Credits,
                      DepartmentName = p.Department.Name
                    }).ToListAsync();

      // Course = await _context.Courses
      //     .Include(c => c.Department)
      //     .AsNoTracking().ToListAsync();

    }
  }
}
