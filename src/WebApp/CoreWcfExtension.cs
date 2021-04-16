using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWCF.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace WebApp
{
    public static class ServiceModelWebHostBuilderExtensions
    {
        public static IHostBuilder UseNetTcp(this IHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseNetTcp(808);
        }

        public static IHostBuilder UseNetTcp(this IHostBuilder webHostBuilder, int port)
        {
            // Using default port
            webHostBuilder.ConfigureServices(services =>
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportServiceBuilder, NetTcpTransportServiceBuilder>());
                services.AddSingleton(NetMessageFramingConnectionHandler.BuildAddressTable);
                services.AddNetTcpServices(new IPEndPoint(IPAddress.Any, port));
                services.AddTransient<IFramingConnectionHandshakeBuilder, FramingConnectionHandshakeBuilder>();
            });

            return webHostBuilder;
        }

        private static IServiceCollection AddNetTcpServices(this IServiceCollection services, IPEndPoint endPoint)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<KestrelServerOptions>, NetTcpFramingOptionsSetup>());
            services.TryAddSingleton<NetMessageFramingConnectionHandler>();
            services.Configure<NetTcpFramingOptions>(o =>
            {
                o.EndPoints.Add(endPoint);
            });

            return services;
        }
    }
}
