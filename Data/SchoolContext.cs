using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Models {
  public class SchoolContext : DbContext {
    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options) {
    }

    public DbSet<Student> Student { get; set; }
    public DbSet<Enrollment> Enrollment { get; set; }
    public DbSet<Course> Course { get; set; }

    // DbSet<Enrollment> and DbSet<Course> could be omitted. EF Core includes them implicitly because the Student entity references the Enrollment entity, and the Enrollment entity references the Course entity. 
  }
}
