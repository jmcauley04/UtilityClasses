namespace UtilityClasses;

/// <summary>
/// Sometimes the amount of data that needs to be acted on is too large and needs to be batched.  
/// This helper function handles that scenario.
/// </summary>
public class BatchHandler
{
    public Action<int>? UpdatePercentComplete { get; set; }

    public async Task BatchAndExecute<T>(IEnumerable<T> items, int batchSize, Func<IEnumerable<T>, Task> func)
    {
        var itemList = items.ToList();
        var chunkQty = (int)Math.Ceiling(itemList.Count * 1.0 / batchSize);

        var iteration = 0;
        foreach (var chunk in Chunk(items.ToList(), batchSize))
        {
            await func.Invoke(chunk);

            UpdatePercentComplete?.Invoke(++iteration * 100 / chunkQty);
        }
    }

    private static IEnumerable<IEnumerable<T>> Chunk<T>(IList<T> items, int size)
    {
        int iteration = 0;

        while (iteration * size < items.Count)
        {
            yield return items
                .Skip(size * iteration++)
                .Take(size);
        }
    }

}
