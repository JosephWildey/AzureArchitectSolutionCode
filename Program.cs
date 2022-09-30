using System.Threading.Tasks;    
using Azure.Messaging.ServiceBus;


class Program{
// connection string to your Service Bus namespace
static string connectionString = "Endpoint=sb://az204svcbus22218.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OH0RmS4Won06vinAJSXnpwF6JdBdh1kaqtyxPaZvLlg=";

// name of your Service Bus topic
static string queueName = "az204-queue";

// the client that owns the connection and can be used to create senders and receivers
static ServiceBusClient client = new ServiceBusClient(connectionString);
// the sender used to publish messages to the queue
static ServiceBusSender sender = client.CreateSender(queueName);
// the processor that reads and processes messages from the queue
static ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
// number of messages to be sent to the queue
private const int numOfMessages = 3;

// handle received messages
static async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // complete the message. messages is deleted from the queue. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

static async Task Main()
{
    // Create the client object that will be used to create sender and receiver objects
    client = new ServiceBusClient(connectionString);

    try
    {
        // add handler to process messages
        processor.ProcessMessageAsync += MessageHandler;

        // add handler to process any errors
        processor.ProcessErrorAsync += ErrorHandler;

        // start processing 
        await processor.StartProcessingAsync();

        Console.WriteLine("Wait for a minute and then press any key to end the processing");
        Console.ReadKey();

        // stop processing 
        Console.WriteLine("\nStopping the receiver...");
        await processor.StopProcessingAsync();
        Console.WriteLine("Stopped receiving messages");
    }
    finally
    {
        // Calling DisposeAsync on client types is required to ensure that network
        // resources and other unmanaged objects are properly cleaned up.
        await processor.DisposeAsync();
        await client.DisposeAsync();
    }
}
}