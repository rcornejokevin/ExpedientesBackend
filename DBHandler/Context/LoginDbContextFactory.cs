using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DBHandler.Context
{
    public class LoginDbContextFactory : IDesignTimeDbContextFactory<LoginDbContext>
    {
        public LoginDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("OracleDBUser");

            var optionsBuilder = new DbContextOptionsBuilder<LoginDbContext>();
            optionsBuilder.UseOracle(connectionString);

            return new LoginDbContext(optionsBuilder.Options, configuration);
        }
    }
}
