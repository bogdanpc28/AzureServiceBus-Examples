using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Queue;

internal static class Program
{
    private const string ServiceBusConnectionString =
        "Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AbCdEfGhIjKlMnOpQrStUvWxYz==";
    private const string QueueName = "salesmessages";

    private static void Main(string[] args)
    {
        Console.WriteLine("Sending a message to the Sales Messages queue...");
        SendSalesMessageAsync().GetAwaiter().GetResult();
        Console.WriteLine("Message was sent successfully.");
    }

    private static async Task SendSalesMessageAsync()
    {
        await using var client = new ServiceBusClient(ServiceBusConnectionString);

        await using var sender = client.CreateSender(QueueName);
        try
        {
            const string messageBody = "$10,000 order for bicycle parts from retailer Adventure Works.";
            var message = new ServiceBusMessage(messageBody);
            Console.WriteLine($"Sending message: {messageBody}");
            await sender.SendMessageAsync(message);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}