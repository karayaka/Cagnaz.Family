using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cagnaz.Family.Persistence.DataContexts;

public class DesignTimeDbContextFactory:IDesignTimeDbContextFactory<MySqlFamilyDataContext>
{
    public MySqlFamilyDataContext CreateDbContext(string[] args)
    {
        var serverVersion = new MySqlServerVersion(new Version(10, 3, 35));
        var builder = new DbContextOptionsBuilder<MySqlFamilyDataContext>();
        var connectionString = "server=213.238.183.232;port=3306;database=cagnazco_family_test;user=cagnazco_family_test;password=Kara.531531!";//test
        //var connectionString = "server=213.238.183.232;port=3306;database=cagnazco_family;user=cagnazco_family;password=Kara.531531!";//prod
        builder.UseMySql(
            connectionString,
            serverVersion,
            options => options.EnableRetryOnFailure()
        );
        return new MySqlFamilyDataContext(builder.Options);
    }
}