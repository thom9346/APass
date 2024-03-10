using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PasswordManagerContext>
    {
        public PasswordManagerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PasswordManagerContext>();
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "APass", "passwordManager.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new PasswordManagerContext(optionsBuilder.Options);
        }
    }
}
