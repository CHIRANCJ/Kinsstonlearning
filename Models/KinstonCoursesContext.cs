using KinstonLearning.Models;
using Microsoft.EntityFrameworkCore;

public class KinstonCoursesContext : DbContext
{
    public KinstonCoursesContext(DbContextOptions<KinstonCoursesContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<ModuleCompletion> ModuleCompletions { get; set; }
    public DbSet<CourseCompletion> CourseCompletions { get; set; }
    public DbSet<CourseChangeRequest> CourseChangeRequests { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the foreign key relationship for CourseChangeRequest
        modelBuilder.Entity<CourseChangeRequest>()
            .HasOne(c => c.RequestedByProfessor)
            .WithMany()
            .HasForeignKey(c => c.RequestedByProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the foreign key relationship for Course
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Professor)
            .WithMany()
            .HasForeignKey(c => c.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Other model configurations can go here...
    }
}
