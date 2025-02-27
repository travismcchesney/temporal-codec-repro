using System.Text;
using System.Threading.Tasks;

namespace TemporalioSamples.ActivitySimple;

using Temporalio.Activities;

public class MyActivities
{
    // Activities can be static and/or sync
    [Activity]
    public static string DoStaticThing() => "some-static-value";

    // Activities can be methods that can access state
    [Activity]
    public Task<string> DoAsyncThing() =>
        Task.FromResult("Hello from Async!");
}
