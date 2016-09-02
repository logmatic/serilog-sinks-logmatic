# Serilog.Sinks.Logmatic

**ADD NUGET STATUS** / **ADD BUILD STATUS**

A Serilog sink that send events and logs staight away to [Logmatic.io](http://logmatic.io).

By default the sink will use a TCP token over SSL.

**Package** - [Serilog.Sinks.Logmatic](http://nuget.org/packages/serilog.sinks.logmatic)
| **Platforms** - .NET 4.5

```csharp

var log = new LoggerConfiguration()
    .WriteTo.Logmatic("<API_KEY>")
    .CreateLogger();
```

