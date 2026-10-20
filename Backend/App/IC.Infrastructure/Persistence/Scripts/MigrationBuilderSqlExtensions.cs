using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IC.Infrastructure.Persistence.Scripts
{
    public static class MigrationBuilderSqlExtensions
    {
        private static readonly Assembly ScriptAssembly = typeof(MigrationBuilderSqlExtensions).Assembly;

        /// <summary>
        /// Applies an embedded .sql file from Persistence/Scripts.
        /// Example: <c>migrationBuilder.ApplySqlScript("Views/OpenIncidents_View.sql");</c>
        /// Scripts must be re-runnable (CREATE OR ALTER / MERGE) and must not contain GO.
        /// </summary>
        /// <param name="wrapInExec">
        /// Wraps the script in EXEC so CREATE VIEW/PROCEDURE/FUNCTION/TRIGGER can run
        /// inside the migration batch. Set to false for seed scripts.
        /// </param>
        public static MigrationBuilder ApplySqlScript(this MigrationBuilder migrationBuilder, string relativePath, bool wrapInExec = true)
        {
            string resourceSuffix = ".Persistence.Scripts." + relativePath.Replace('/', '.').Replace('\\', '.');
            string resourceName = ScriptAssembly.GetManifestResourceNames()
                .SingleOrDefault(n => n.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException($"Embedded SQL script not found: {relativePath}");

            using Stream stream = ScriptAssembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            string sql = reader.ReadToEnd().Trim();

            migrationBuilder.Sql(wrapInExec ? $"EXEC(N'{sql.Replace("'", "''")}');" : sql);
            return migrationBuilder;
        }
    }
}
