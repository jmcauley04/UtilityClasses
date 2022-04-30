using UtilityClasses;

Console.WriteLine("Hello, World!");

await RunExample();

Console.WriteLine("Finished");

static async Task RunExample()
{
    // initialize a BatchHandler class
    var batchApiCalls = new BatchHandler();

    // This is an example method that would be used to update the UI
    void updateConsole(int percent) => Console.WriteLine($"\nWe are {percent}% completed");

    // This is an example method that acts on each chunk of data
    async Task myTask(IEnumerable<int> items)
    {
        Console.Write($"\n\nStarting {items.Count()} items with range {items.First()} to {items.Last()}... ");
        await Task.Delay(1500);
        Console.WriteLine("Complete!");
    }

    // This is subscribing the progress reports to the method that updates the UI
    batchApiCalls.UpdatePercentComplete += updateConsole;

    // an example collection of data and batch size
    var items = Enumerable.Range(0, 1000);
    var batchSize = 72;

    try
    {
        // This is the workhorse of the operation
        // The method will chunk the data into, at most, the batch size and perform the task on each
        // batch of data
        await batchApiCalls.BatchAndExecute(items, batchSize, myTask);
    }
    finally
    {
        // This is unsubscribing the progress reports to the method that updates the UI
        batchApiCalls.UpdatePercentComplete -= updateConsole;
    }
}