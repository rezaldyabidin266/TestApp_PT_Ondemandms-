using Microsoft.EntityFrameworkCore;

namespace TestAppOnde.Models
{
    public class DbContextSql : DbContext
    {
        public DbContextSql(DbContextOptions<DbContextSql> options) : base(options) { }
        public DbSet<TransaksiModels> transaksiContext { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TransaksiModels>().ToTable("Tb_Transaksi");
        }

    }
}
