using Microsoft.EntityFrameworkCore;
using TestExerciseSK.DAL;
using System.Data.SQLite;

namespace TestExerciseUnitTestsSK.Context
{
    public class SqlLite_UnitTests
    {
        private DbContextOptions<TestExerciseSKDbContext> options;

        public SqlLite_UnitTests()
        {
            options = GetDbContextOptions;
        }

        public TestExerciseSKDbContext GetDbContext()
        {
            var context = new TestExerciseSKDbContext(options);
            // Crea y abre el 'schema' en la base de datos
            context.Database.EnsureCreated();
            return context;
        }

        private DbContextOptions<TestExerciseSKDbContext> GetDbContextOptions
        {
            get
            {
                // La BD in-memory solo existe cuando la conexión está abierta
                var connection = new SQLiteConnection("DataSource=:memory:");
                connection.Open();

                var options = new DbContextOptionsBuilder<TestExerciseSKDbContext>()
                        .UseSqlite(connection)
                        .Options;

                return options;
            }
        }
    }
}
