using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments {
  public class EditModel : PageModel {
    private readonly ContosoUniversity.Models.SchoolContext _context;

    public EditModel(ContosoUniversity.Models.SchoolContext context) {
      _context = context;
    }

    [BindProperty]
    public Department Department { get; set; }
    // replace ViewData["InstructorID"]
    public SelectList InstructorNameSL { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id) {
      Department = await _context.Departments
                  .Include(d => d.Administrator)    // eager loading
                  .AsNoTracking()                 // tracking not required
                  .FirstOrDefaultAsync(m => m.DepartmentID == id);

      if (Department == null) {
        return NotFound();
      }

      //ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FirstMidName");

      // use strongly typed data rather then ViewData
      InstructorNameSL = new SelectList(_context.Instructors, "ID", "FirstMidName");

      return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id) {
      if (!ModelState.IsValid) {
        return Page();
      }

      var departmentToUpdate = await _context.Departments
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

      // null means Department was deleted by another user
      if (departmentToUpdate == null) {
        return HandleDeletedDepartment();
      }

      // Update the RowVersion with the value when this entity was fetched
      // If the entity has been updated after it was fetched
      //  RowVersion won't match the DB RowVersion and a
      //  DbUpdateConcurrencyException is thrown
      // A second postback will make them match, unless a new concurrency
      //  issue happens
      _context.Entry(departmentToUpdate)
            .Property("RowVersion").OriginalValue = Department.RowVersion;

      if (await TryUpdateModelAsync<Department>(departmentToUpdate, "department",
            s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID)) {
        try {
          await _context.SaveChangesAsync();
          return RedirectToPage("./Index");
        } catch (DbUpdateConcurrencyException ex) {
          var exceptionEntry = ex.Entries.Single();
          var clientValues = (Department)exceptionEntry.Entity;

          var databaseEntry = exceptionEntry.GetDatabaseValues();
          if (databaseEntry == null) {
            ModelState.AddModelError(string.Empty, "Unable to save. " +
                  "The department was deleted by another user.");
            return Page();
          }

          var dbValues = (Department)databaseEntry.ToObject();
          await setDbErrorMessage(dbValues, clientValues, _context);

          // Set the current RowVersion so next postback matches
          //  unless a new concurrency issue happens.
          Department.RowVersion = (byte[])dbValues.RowVersion;
          // must clear the model error for the next postback
          ModelState.Remove("Department.RowVersion");
        }
      }

      InstructorNameSL = new SelectList(_context.Instructors,
            "ID", "FullName", departmentToUpdate.InstructorID);

      return Page();


      // _context.Attach(Department).State = EntityState.Modified;

      // try {
      //   await _context.SaveChangesAsync();
      // } catch (DbUpdateConcurrencyException) {
      //   if (!DepartmentExists(Department.DepartmentID)) {
      //     return NotFound();
      //   } else {
      //     throw;
      //   }
      // }

      // return RedirectToPage("./Index");
    }

    private IActionResult HandleDeletedDepartment() {
      var deletedDepartment = new Department();
      // ModelState contains the posted data because of the deletion error and
      //  will override the Department instance values whe displaying Page()
      ModelState.AddModelError(string.Empty,
            "Unable to save. The department was deleted by another user.");
      InstructorNameSL = new SelectList(_context.Instructors,
            "ID", "FullName", Department.InstructorID);

      return Page();
    }

    private async Task setDbErrorMessage(Department dbValues, Department clientValues, SchoolContext context) {

      if (dbValues.Name != clientValues.Name) {
        ModelState.AddModelError("Department.Name", $"Current Value: {dbValues.Name}");
      }
      if (dbValues.Budget != clientValues.Budget) {
        ModelState.AddModelError("Department.Budget", $"Current Value: {dbValues.Budget:c}");
      }
      if (dbValues.StartDate != clientValues.StartDate) {
        ModelState.AddModelError("Department.StartDate",
              $"Current Value: {dbValues.StartDate:d}");
      }
      if (dbValues.InstructorID != clientValues.InstructorID) {
        Instructor dbInstructor = await _context.Instructors.FindAsync(dbValues.InstructorID);
        ModelState.AddModelError("Department.InstructorID",
              $"Current Value: {dbInstructor.FullName}");
      }
      ModelState.AddModelError(string.Empty,
            "The record you attempeted to edit " +
            "was modified by anothe user after you./n" +
            "The edit operation was cancelled and " +
            "the current values in the database have been dispalyed./n" +
            "If you still want to edit this record, " +
            "click the Save button again.");
    }

    private bool DepartmentExists(int id) {
      return _context.Departments.Any(e => e.DepartmentID == id);
    }
  }
}
