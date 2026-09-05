using System.Collections.Frozen;

namespace MediaRelay.Console.Extensions;


internal static class FrozenDictionaryExtensions
{
    extension<TKey, TValue>(FrozenDictionary<TKey, TValue>)
        where TKey: notnull
    {
        public static FrozenDictionary<TKey, TValue> Empty => EmptyFrozenDictionary<TKey, TValue>.Empty;
    }
}


file static class EmptyFrozenDictionary<TKey, TValue>
        where TKey : notnull
{
    public static FrozenDictionary<TKey, TValue> Empty { get; } = new Dictionary<TKey, TValue>().ToFrozenDictionary();
}