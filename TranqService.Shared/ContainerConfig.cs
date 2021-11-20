namespace TranqService.Shared;
public static class ContainerConfig
{
    public static void Configure(ContainerBuilder builder)
    {
        // Bulk registrations
        builder.BulkRegister(
            "TranqService.Shared.ApiHandlers",  // Register Api handlers
            "TranqService.Shared.Data",
            "TranqService.Shared.DataAccess",
            "TranqService.Shared.Logic"
        );
    }

    /// <summary>
    /// Registers all classes which have a corresponding IClassName interface in the same namespace.
    /// </summary>
    /// <param name="af"></param>
    /// <param name="assemblyPath">ideally use nameof() to select namespace</param>
    public static void BulkRegister(this ContainerBuilder af, params string[] assemblyPaths)
    {
        var allTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes();

        foreach (string assemblyPath in assemblyPaths)
            foreach (Type t in allTypes)
            {
                string owo = t.Name;

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

                // Register
                af.RegisterType(t)
                    .As(correspondingInterfaces.First());
            }
    }
}