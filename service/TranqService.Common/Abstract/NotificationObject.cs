namespace TranqService.Common.Abstract;

public abstract class NotificationObject : INotifyPropertyChanged
{
    private ConcurrentDictionary<string, object?> FieldData = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged<T>(Expression<Func<T>> me)
        => RaisePropertyChanged((me.Body as MemberExpression)?.Member.Name ?? "BROKEN");

    protected virtual void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new(propertyName));

    /// <summary>
    /// Updates a property. 
    /// Fires PropertyChanged if the property is found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    protected void Set<T>(string propertyName, T value)
    {
        FieldData[propertyName] = value;
        RaisePropertyChanged(propertyName);
    }

    /// <summary>
    /// Get a property if it exists. Null is allowed. Default is default(T).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected T? Get<T>(string propertyName, T overrideDefault = default)
        => FieldData.TryGetValue(propertyName, out var data)
        ? (T?)data : overrideDefault;
}