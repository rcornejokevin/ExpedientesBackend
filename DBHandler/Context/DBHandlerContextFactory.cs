using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DBHandler.Context
{
    public class DBHandlerContextFactory : IDesignTimeDbContextFactory<DBHandlerContext>
    {
        public DBHandlerContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("OracleDB");

            var optionsBuilder = new DbContextOptionsBuilder<DBHandlerContext>();
            optionsBuilder.UseOracle(connectionString);

            return new DBHandlerContext(optionsBuilder.Options, configuration);
        }
    }
}