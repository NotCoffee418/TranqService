using Autofac;
using Autofac.Extensions.DependencyInjection;
using TranqService.Shared;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<YoutubeDownloadService>();
    })
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(ContainerConfig.Configure)
    .Build();

await host.RunAsync();
