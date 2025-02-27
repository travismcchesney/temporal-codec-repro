using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using Temporalio.Converters;
using Temporalio.Worker;
using TemporalioSamples.ActivitySimple;

var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").SetMinimumLevel(LogLevel.Information));

// Create a client to localhost on default namespace
var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions("localhost:7233")
{
    DataConverter = DataConverter.Default with
    {
        PayloadCodec = new NoopCodec()
    },
    LoggerFactory = loggerFactory
});

switch (args.ElementAtOrDefault(0))
{
    case "worker":
        await RunWorkerAsync();
        break;
    case "workflow":
        await ExecuteWorkflowAsync();
        break;
    default:
        throw new ArgumentException("Must pass 'worker' or 'workflow' as the single argument");
}

return;

async Task ExecuteWorkflowAsync()
{
    Console.WriteLine("Executing workflows");
    List<Task> tasks = [];
    
    tasks.Add(client.ExecuteWorkflowAsync(
        (MyWorkflow wf) => wf.RunAsync(),
        new WorkflowOptions(id: "activity-simple-workflow-id", taskQueue: "activity-simple-sample")));
    
    await Task.WhenAll(tasks);
}

async Task RunWorkerAsync()
{
    // Cancellation token cancelled on ctrl+c
    using var tokenSource = new CancellationTokenSource();
    Console.CancelKeyPress += (_, eventArgs) =>
    {
        tokenSource.Cancel();
        eventArgs.Cancel = true;
    };

    // Create an activity instance with some state
    var activities = new MyActivities();

    // Run worker until cancelled
    Console.WriteLine("Running worker");
    using var worker = new TemporalWorker(
        client,
        new TemporalWorkerOptions(taskQueue: "activity-simple-sample").AddActivity(activities.DoAsyncThing)
            .AddActivity(MyActivities.DoStaticThing).AddWorkflow<MyWorkflow>());
    try
    {
        await worker.ExecuteAsync(tokenSource.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Worker cancelled");
    }
}
