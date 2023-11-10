namespace TranqService.Common.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Register all classes with a corresponding interface in an assembly that start with a path defined in assemblyPaths
    /// </summary>
    /// <param name="af"></param>
    /// <param name="assemblyPaths"></param>
    /// <exception cref="Exception"></exception>
    public static void BulkRegister(this ContainerBuilder af, params string[] assemblyPaths)
    {
        assemblyPaths = assemblyPaths.Where(x => x.Length > 0).ToArray();
        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetReferencedAssemblies())
            .DistinctBy(x => x.FullName)
            // Get all assemblies starting with anything from assemblyPaths
            .Where(a => assemblyPaths.Select(x => a.FullName.StartsWith(x)).Where(x => x).Any())
            .Select(a => Assembly.Load(a))
            .SelectMany(t => t.GetTypes())
            .ToList();

        foreach (string assemblyPath in assemblyPaths)
            foreach (Type t in allTypes)
            {
                // False if ineligible
                bool isIneligible = t.Namespace is null || // Defined namespace
                                    !t.Namespace.StartsWith(assemblyPath) || // matches assemblyPath
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
                var typeRegistration = af.RegisterType(t)
                    .As(correspondingInterfaces.First());

                // Extract scope info
                DependencyScopeAttribute? scope = t.GetCustomAttribute<DependencyScopeAttribute>();
                if (scope is null || scope.ScopeDefinition == Scope.Undefined) // Default
                    typeRegistration.InstancePerLifetimeScope();
                else if (scope.ScopeDefinition == Scope.PerDependency)
                    typeRegistration.InstancePerDependency();
                else if (scope.ScopeDefinition == Scope.Single)
                    typeRegistration.SingleInstance();
                else if (scope.ScopeDefinition == Scope.Lifetime)
                    typeRegistration.InstancePerLifetimeScope();
                else throw new Exception("BulkRegister does not know how to handle scope " + scope.ScopeDefinition);
            }
    }
}
