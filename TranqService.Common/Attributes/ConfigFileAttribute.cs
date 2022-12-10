using TranqService.Common.Data;

namespace TranqService.Common.Attributes;

internal class ConfigFileAttribute : Attribute
{
    /// <summary>
    /// Specifies which config file to use for the implementing class
    /// </summary>
    /// <param name="configFileName">Only filename or path inside of appdata/local/tranqservice</param>
    public ConfigFileAttribute(string configFileName)
	{
        ConfigFilePath = PathHelper.GetAppdataPath(true, configFileName);
	}

    private string ConfigFilePath { get; init; }

    /// <summary>
    /// Extract full path from the attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception">Attribute must not be missing on implementation of ConfigBase<T></exception>
    public static string GetConfigFilePath<T>()
        where T : class, new()
    {
        // Extract config file from attribute
        var attributes = typeof(T).GetCustomAttributes(typeof(ConfigFileAttribute), true);
        if (attributes.Length == 0)
            throw new Exception("Config file attribute not found for type " + typeof(T).Name);

        // Extract it
        return (attributes[0] as ConfigFileAttribute).ConfigFilePath;
    }
}
