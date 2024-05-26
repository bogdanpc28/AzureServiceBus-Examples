using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.TopicSender;

internal static class Program
{
    private const string ServiceBusConnectionString =
        "Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AbCdEfGhIjKlMnOpQrStUvWxYz==";
    private const string TopicName = "salesperformancemessages";

    private static void Main(string[] args)
    {
        Console.WriteLine("Sending a message to the Sales Performance topic...");
        SendPerformanceMessageAsync().GetAwaiter().GetResult();
        Console.WriteLine("Message was sent successfully.");
    }

    private static async Task SendPerformanceMessageAsync()
    {
        // By leveraging "await using", the DisposeAsync method will be called automatically once the client variable goes out of scope.
        // In more realistic scenarios, you would store off a class reference to the client (rather than to a local variable) so that it can be used throughout your program.
        await using var client = new ServiceBusClient(ServiceBusConnectionString);

        await using var sender = client.CreateSender(TopicName);

        try
        {
            const string messageBody = "Total sales for Brazil in August: $13m.";
            var message = new ServiceBusMessage(messageBody);
            Console.WriteLine($"Sending message: {messageBody}");
            await sender.SendMessageAsync(message);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
        }
    }
}