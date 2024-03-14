using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace APass.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PasswordManagerContext>
    {
        public PasswordManagerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PasswordManagerContext>();
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "APass", "passwordManager_APass_SSD.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new PasswordManagerContext(optionsBuilder.Options);
        }
    }
}
