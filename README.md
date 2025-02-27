# temporal-codec-repro
Reproduction of .NET payload codec issue

This small example includes a very small workflow with a `NoopCodec`. The codec does nothing on encode and decode other than return the original payload (without using `.Clone()`) or anything. To run the example:

```
temporal server start-dev

dotnet run worker

dotnet run workflow
```
