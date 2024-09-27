using Microsoft.EntityFrameworkCore;

namespace RestApi.Models
{
    public class ApiDemoDbContext: DbContext
    {
        public ApiDemoDbContext(DbContextOptions<ApiDemoDbContext> options) : base(options)
        {

        }
        public DbSet<Students> Students { get; set; }
        public DbSet<Courses> Courses { get; set; }

      
    }
}
