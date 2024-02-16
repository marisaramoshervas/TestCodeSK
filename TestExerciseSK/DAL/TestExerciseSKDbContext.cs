using Microsoft.EntityFrameworkCore;
using TestExerciseSK.Models;

namespace TestExerciseSK.DAL
{
    public class TestExerciseSKDbContext : DbContext
    {
        public TestExerciseSKDbContext(DbContextOptions<TestExerciseSKDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
    
}
