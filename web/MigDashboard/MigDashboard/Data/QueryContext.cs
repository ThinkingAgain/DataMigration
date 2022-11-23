using Microsoft.EntityFrameworkCore;
using MigDashboard.Models;

namespace MigDashboard.Data
{
    public class QueryContext : DbContext
    {
        public QueryContext(DbContextOptions<QueryContext> options) : base(options)
        {

        }

        public DbSet<QueryConfig> QueyConfigs { get; set; }
    }
}
