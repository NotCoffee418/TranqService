namespace TranqService.Common.Attributes;

/// <summary>
///     Used by BulkRegister to define the scope of an injectable implementation
/// </summary>
public class DependencyScopeAttribute : Attribute
{
    public DependencyScopeAttribute(Scope scopeDef)
    {
        ScopeDefinition = scopeDef;
    }

    public Scope ScopeDefinition { get; set; }

    // Used by DependencyScopeAttributes
    public enum Scope
    {
        Undefined = 0,

        // Every injection new instance
        PerDependency = 1,

        // one instance is returned from all requests in the root and all nested scopes
        Single = 2,

        // One lifetime instance
        Lifetime = 3
    }
}
