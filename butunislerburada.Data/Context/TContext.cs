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

        public DbSet<Admin> Admin { get; set; }

        public DbSet<Singer> Singer { get; set; }
        public DbSet<Lyrics> Lyrics { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<BadLinkLog> BadLinkLog { get; set; }

        public DbSet<Contact> Contact { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<RecentTransaction> RecentTransaction { get; set; }

        public DbSet<Setting> Setting { get; set; }
    }
}
