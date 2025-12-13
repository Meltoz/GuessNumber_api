using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Meltix.IntegrationTests
{
    public static class DbContextProvider
    {
        public static GuessNumberContext SetupContext(SqliteConnection? connection = null)
        {
            connection ??= new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GuessNumberContext>()
                .UseSqlite(connection)
                .Options;

            var context = new GuessNumberContext(options, true);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
