﻿using FakturowniaService;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Microsoft.Extensions.DependencyInjection;
using FakturowniaService.task;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using FakturowniaService.util;
using System.Configuration.Install;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Collections.Generic;

namespace FakturExport
{
    internal class Program
    {
        private static readonly String serviceName = "FakturService";
        private static readonly String serviceVersion = "1.0.0";

        private static void ConfigureServices(HostApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "VIR Faktur ETL service.";
            });

            appBuilder.Services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .AddSource(serviceName)
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                            serviceName: serviceName,
                            serviceVersion: serviceVersion))
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter();
                })
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                            serviceName: serviceName,
                            serviceVersion: serviceVersion))
                        .AddMeter(serviceName)
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter();
                        //.AddConsoleExporter();
                });

            appBuilder.Services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                        serviceName: serviceName,
                        serviceVersion: serviceVersion));
                    options.AddOtlpExporter();
                });

                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string log4NetConfigFilePath = Path.Combine(exeDirectory, "log4net.config");
                builder.AddLog4Net(log4NetConfigFilePath);
            });

            appBuilder.Services.AddSingleton<MetricsService>();

            var assembly = Assembly.GetExecutingAssembly();

            
            // Register Faktur tasks
            var fakturTasks = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ETLTask).IsAssignableFrom(t) &&
                            t.GetCustomAttribute<FakturTaskAttribute>() != null);
            foreach (var task in fakturTasks)
            {
                appBuilder.Services.AddTransient(typeof(ETLTask), task);
            }

            // Register JobStatus tasks
            var jobStatusTasks = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ETLTask).IsAssignableFrom(t) &&
                            t.GetCustomAttribute<JobStatusTaskAttribute>() != null);
            foreach (var task in jobStatusTasks)
            {
                appBuilder.Services.AddTransient(typeof(ETLTask), task);
            }
            
            appBuilder.Services.AddHostedService(sp =>
                new FakturService(sp.GetRequiredService< ILogger < FakturService >>(), sp.GetRequiredService<IEnumerable<ETLTask>>()
                    .Where(t => t.GetType().GetCustomAttribute<FakturTaskAttribute>() != null)));

            appBuilder.Services.AddHostedService(sp =>
                new JobStatusService(sp.GetRequiredService<ILogger<JobStatusService>>(), sp.GetRequiredService<IEnumerable<ETLTask>>()
                    .Where(t => t.GetType().GetCustomAttribute<JobStatusTaskAttribute>() != null)));

            appBuilder.Services.AddSingleton(sp =>
                new JobStatusService(sp.GetRequiredService<ILogger<JobStatusService>>(), sp.GetRequiredService<IEnumerable<ETLTask>>()
                    .Where(t => t.GetType().GetCustomAttribute<JobStatusTaskAttribute>() != null)));

        }

        static void Main(string[] args)
        {

            //var services = new ServiceCollection();
            HostApplicationBuilder appBuilder = Host.CreateApplicationBuilder(args);

            ConfigureServices(appBuilder);

            var serviceProvider = appBuilder.Services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogDebug("Framework: " + FRWK.GetEnvironmentVersion() + " " + FRWK.GetTargetFrameworkName() + " " + FRWK.GetFrameworkDescription());

            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default:
                        // Run as a console app for debugging

                        using (logger.BeginScope("Console mode"))
                        {
                            logger.LogInformation("Starting the service in interactive mode.");
                            /*var fakturService = serviceProvider.GetRequiredService<FakturService>();
                            fakturService.StartAsConsole(null);
                            */

                            var jobStatusService = serviceProvider.GetRequiredService<JobStatusService>();
                            jobStatusService.StartAsConsole(null);
                            
                        }
                        break;
                }
            }
            else
            {
                logger.LogInformation("Starting the service as a Windows Service...");
                //ServiceBase[] servicesToRun = new ServiceBase[] { serviceProvider.GetRequiredService<FakturService>() };
                //ServiceBase.Run(servicesToRun);
                IHost host = appBuilder.Build();
                host.Run();
            }

            serviceProvider.Dispose();
        }
    }
}
