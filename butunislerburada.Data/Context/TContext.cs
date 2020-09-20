using butunislerburada.Data.Entity;
using butunislerburada.Data.Migrations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace butunislerburada.Data.Context
{
    public class TContext : DbContext
    {
        public TContext() : base("DbConnection")
        {
            Database.SetInitializer(strategy: new MigrateDatabaseToLatestVersion<TContext, Configuration>("DbConnection"));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Job> Job { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<JobCity> JobCity { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<WorkingWay> WorkingWay { get; set; }
    }
}
