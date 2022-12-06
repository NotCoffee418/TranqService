namespace TranqService.Tests;

internal static class DependencyBuilder
{
    internal static ContainerBuilder AddMocked(this ContainerBuilder builder)
    {
        // Add caleb config
        builder.ConfigureCommon();
        builder.ConfigureShared();
        builder.ConfigureYtdlp();
        //builder.ConfigureDatabase() // dont use literally

        // Mock IConfiguration
        var mockedConf = GetConfigurationUnderTest();
        builder.RegisterInstance(mockedConf);

        return builder;
    }

    private static IConfiguration GetConfigurationUnderTest()
    {
        // "nested:values:can:be:added:like:this"
        var unitTestConfigurationValues = new Dictionary<string, string>
        {
            { "Key:Subkey:Subsubkey", "value" }
        };

        // Build and return
        return new ConfigurationBuilder()
            .AddInMemoryCollection(unitTestConfigurationValues)
            .Build();
    }
}
