namespace TranqService.Common.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers all classes which have a corresponding IClassName interface in the same namespace.
    /// </summary>
    /// <param name="af"></param>
    /// <param name="assemblyPath">ideally use nameof() to select namespace</param>
    public static void BulkRegister(this ContainerBuilder af, params string[] assemblyPaths)
    {
        var allTypes = Assembly.GetEntryAssembly()
            .GetReferencedAssemblies()
            .Where(a => a.FullName.StartsWith("TranqService."))
            .Select(t => Assembly.Load(t))
            .SelectMany(t => t.GetTypes())
            .ToList();

        foreach (string assemblyPath in assemblyPaths)
            foreach (Type t in allTypes)
            {
                // False if ineligible
                bool isIneligible = t.Namespace is null ||  // Defined namespace
                    !t.Namespace.StartsWith(assemblyPath) ||  // matches assemblyPath
                    !t.IsClass || t.IsAbstract || // is not interface
                    !t.GetInterfaces().Any(); // Only contine if it has any interfaces
                if (isIneligible)
                    continue;

                // Attempt to find corresponding interface
                var correspondingInterfaces = t.GetInterfaces()
                    .Where(i => i.Name == $"I{t.Name}");

                // False if no corresponding interface 
                if (correspondingInterfaces.Count() == 0)
                    continue;

                // Begin registration
                af.RegisterType(t)
                    .As(correspondingInterfaces.First())
                    .InstancePerLifetimeScope();
            }
    }
}
