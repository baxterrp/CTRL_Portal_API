using System;
using System.Linq;
using System.Reflection;

namespace CTRL.Portal.Migrations
{
    public class MigrationEngine
    {
        public static Assembly[] GetCustomMigrationAssemblies() => 
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsClass && t.Namespace == "CTRL.Portal.Migrations.Custom")
                .Select(x => x.Assembly).ToArray();
    }
}
