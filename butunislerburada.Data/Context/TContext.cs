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
    }
}
