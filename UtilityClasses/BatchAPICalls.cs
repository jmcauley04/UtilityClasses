namespace UtilityClasses;

/// <summary>
/// Sometimes the amount of data that needs to be sent to an API is too large and needs to be batched.  This helper function
/// handles this scenario.
/// </summary>
public class BatchAPICalls
{
    public Action<int>? UpdatePercentComplete { get; set; }

    public async Task SendToAPI<T>(IEnumerable<T> items, int chunkSize, Func<IEnumerable<T>, Task> func)
    {
        var itemList = items.ToList();
        var chunkQty = (int)Math.Ceiling(itemList.Count * 1.0 / chunkSize);

        var iteration = 0;
        foreach (var chunk in Chunk(items.ToList(), chunkSize))
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


    public static async Task RunExample()
    {
        Console.WriteLine("This example is a 120,000 items list that needs to be sent to the API endpoint.  The amount of data is too large to be sent in a single call.");

        var batchApiCalls = new BatchAPICalls();

        void updateConsole(int percent) => Console.WriteLine($"We are {percent}% completed");

        async Task myTask(IEnumerable<int> items)
        {
            Console.WriteLine($"\n\nStarting {items.Count()} items with range {items.First()} to {items.Last()}...");
            await Task.Delay(1500);
            Console.WriteLine("Range finished...");
        }

        batchApiCalls.UpdatePercentComplete += updateConsole;

        await batchApiCalls.SendToAPI(Enumerable.Range(0, 1000), 72, myTask);

        batchApiCalls.UpdatePercentComplete -= updateConsole;

    }
}
