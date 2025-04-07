namespace Stride.Common;

public abstract class AbstractCache<TCacheValue>
{
    protected List<TCacheValue> Items { get; set; }

    protected const char QuerySeparator = ';';
    protected static readonly int QueryFieldCount = typeof(TCacheValue).GetProperties().Length;
    protected readonly KeyValuePair<CacheResult, TCacheValue?> CacheMissResult = new(CacheResult.Miss, default);
    protected bool HasChanges = false;
    
    public AbstractCache(IEnumerable<TCacheValue> items)
    {
        Items = items.ToList();
    }

    public abstract void Clear();
    
    public abstract KeyValuePair<CacheResult, TCacheValue?> Query(string query);

    public abstract void Add(TCacheValue value);

    public abstract void Flush();

    public abstract void UpdateLocalCache();

    public abstract void Remove(string query);
}

public enum CacheResult
{
    Hit,
    PartialHit,
    Miss
}