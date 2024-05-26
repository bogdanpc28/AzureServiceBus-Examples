using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.QueueReceiver;

internal static class Program
{
    private const string ServiceBusConnectionString =
        "Endpoint=sb://<examplenamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
    private const string QueueName = "salesmessages";

    private static void Main(string[] args)
    {
        ReceiveSalesMessageAsync().GetAwaiter().GetResult();
    }

    private static async Task ReceiveSalesMessageAsync()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
        Console.WriteLine("======================================================");

        var client = new ServiceBusClient(ServiceBusConnectionString);
        var processorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        };

        await using var processor = client.CreateProcessor(QueueName, processorOptions);

        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        await processor.StartProcessingAsync();
        Console.Read();
        await processor.CloseAsync();
    }

    // handle received messages
    private static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

        // complete the message. messages is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}