using AutoFindBot.Helpers;
using JetBrains.Annotations;

namespace AutoFindBot.Controllers.Extensions;

public static class ConcurrentDictionaryExtension
{
    [CollectionAccess(CollectionAccessType.UpdatedContent | CollectionAccessType.Read)]
    public static Task<TValue> GetOrAddAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        [InstantHandle] Func<TKey, Task<TValue>> valueFactory)
    {
        Guard.NotNull(dictionary, nameof (dictionary));
        Guard.NotNull(valueFactory, nameof (valueFactory));
        return dictionary.GetOrAddImplAsync(key, valueFactory);
    }

    [CollectionAccess(CollectionAccessType.UpdatedContent | CollectionAccessType.Read)]
    private static async Task<TValue> GetOrAddImplAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        [InstantHandle] Func<TKey, Task<TValue>> valueFactory)
    {
        TValue orAddImplAsync;
        if (!dictionary.TryGetValue(key, out orAddImplAsync))
        {
            orAddImplAsync = await valueFactory(key).ConfigureAwait(false);
            dictionary.Add(key, orAddImplAsync);
        }
        return orAddImplAsync;
    }
}