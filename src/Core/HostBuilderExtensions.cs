using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace eda.core;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureCustomLogging(this IHostBuilder host, string serviceName) =>
        host
            .ConfigureLogging(log =>
            {
                log.ClearProviders();
                log.AddOpenTelemetry(otel =>
                {
                    otel.SetResourceBuilder(ResourceBuilder.CreateEmpty()
                        .AddService(serviceName));
                    otel.IncludeScopes = true;
                    otel.IncludeFormattedMessage = true;
                    otel.AddConsoleExporter();
                    otel.AddOtlpExporter(a =>
                    {
                        a.Endpoint = new Uri("http://eda-seq:5341/ingest/otlp/v1/logs");
                        a.Protocol = OtlpExportProtocol.HttpProtobuf;
                        // a.Headers = "X-Seq-ApiKey=WBPq4wjBhGll1QlL9m6r";
                    });
                });
            });
}