using ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    public class DBContext : DbContext
    {
        public DbSet<ZhiGongXX> zhiGongXXes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle("Data Source= (DESCRIPTION =     (ADDRESS_LIST =       (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.159)(PORT = 1521))     )     (CONNECT_DATA =       (SERVICE_NAME = orcl)     )   );User ID=mcrp;Password=mcrp;Min Pool Size=0;Max Pool Size=10;Connection Timeout=600;Incr Pool Size=5;Decr Pool Size=2;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
