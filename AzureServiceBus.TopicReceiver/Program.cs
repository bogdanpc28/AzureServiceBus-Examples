using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.TopicReceiver;

internal static class Program
{
    private const string ServiceBusConnectionString =
        "Endpoint=sb://alexgeddyneil.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LIWIyxs8baqQ0bRf5zJLef6OTfrv0kBEDxFM/ML37Zs=";
    private const string TopicName = "salesperformancemessages";
    private const string SubscriptionName = "Americas";

    private static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        var client = new ServiceBusClient(ServiceBusConnectionString);

        Console.WriteLine("======================================================");
        Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
        Console.WriteLine("======================================================");

        var processorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        };

        var processor = client.CreateProcessor(TopicName, SubscriptionName, processorOptions);

        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        await processor.StartProcessingAsync();

        Console.Read();

        await processor.DisposeAsync();
        await client.DisposeAsync();
    }

    private static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        Console.WriteLine($"Received message: SequenceNumber:{args.Message.SequenceNumber} Body:{args.Message.Body}");
        await args.CompleteMessageAsync(args.Message);
    }

    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Message handler encountered an exception {args.Exception}.");
        Console.WriteLine("Exception context for troubleshooting:");
        Console.WriteLine($"- Endpoint: {args.FullyQualifiedNamespace}");
        Console.WriteLine($"- Entity Path: {args.EntityPath}");
        Console.WriteLine($"- Executing Action: {args.ErrorSource}");
        return Task.CompletedTask;
    }
}