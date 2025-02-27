using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Temporalio.Api.Common.V1;
using Temporalio.Converters;

namespace TemporalioSamples.ActivitySimple;

public sealed class NoopCodec : IPayloadCodec
{
    public Task<IReadOnlyCollection<Payload>> EncodeAsync(IReadOnlyCollection<Payload> payloads)
    {
        return Task.FromResult<IReadOnlyCollection<Payload>>
            (payloads.Select(payload => payload).ToList());
    }

    public Task<IReadOnlyCollection<Payload>> DecodeAsync(IReadOnlyCollection<Payload> payloads)
    {
        return Task.FromResult<IReadOnlyCollection<Payload>>
            (payloads.Select(payload => payload).ToList());
    }
    
    private static IReadOnlyCollection<Payload> TransformPayloads(
        IReadOnlyCollection<Payload> payloads,
        Func<Payload, Task<Payload>> transformPayload
    ) => Task.WhenAll(payloads.Select(transformPayload)).Result.ToList();

    private async Task<Payload> EncodePayloadData(Payload payload)
    {
        return await Task.FromResult(payload);
    }

    private async Task<Payload> DecodePayloadData(Payload payload)
    {
        return await Task.FromResult(payload);
    }
}
