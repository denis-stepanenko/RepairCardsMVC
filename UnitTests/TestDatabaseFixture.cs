using Microsoft.EntityFrameworkCore;
using RepairCardsMVC;

namespace UnitTests
{
    public class TestDatabaseFixture
    {
        private const string connectionString = "Server = ; Database = ; Integrated security = true; TrustServerCertificate = True;";

        public ApplicationDbContext CreateContext()
            => new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options);
    }
}
