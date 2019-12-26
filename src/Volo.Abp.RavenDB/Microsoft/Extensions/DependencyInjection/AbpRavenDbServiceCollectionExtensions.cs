using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using System;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp.RavenDB.Volo.Abp.RavenDB;

namespace Volo.Abp.RavenDB.Microsoft.Extensions.DependencyInjection
{
    public static class AbpRavenDbServiceCollectionExtensions
    {
        public static IServiceCollection AddRavenDbAsyncSession(
            this IServiceCollection services,
            Action<RavenOptions> options)
        {
            services.ConfigureOptions<RavenOptionsSetup>();
            services.Configure(options);
            services.AddSingleton(sp =>
            {
                var setup = sp.GetRequiredService<IOptions<RavenOptions>>().Value;
                return setup.GetDocumentStore(setup.BeforeInitializeDocStore);
            });
            return services.AddScoped(sp => sp.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        }
        public static IServiceCollection AddRavenDbAsyncSession(
            this IServiceCollection services,
            RavenSettings settings)
        {
            var store = new DocumentStore()
            {
                Database = settings.DatabaseName,
                Urls = settings.Urls,
                Certificate = string.IsNullOrEmpty(settings.CertFilePath) ? null : new X509Certificate2(settings.CertFilePath, settings.CertPassword)
            };
            store.Initialize();
            services.AddSingleton<IDocumentStore>(store);
            return services.AddScoped(sp => sp.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        }
        public static IServiceCollection AddRavenDbSession(
            this IServiceCollection services,
            Action<RavenOptions> options)
        {
            services.ConfigureOptions<RavenOptionsSetup>();
            services.Configure(options);
            services.AddSingleton(sp =>
            {
                var setup = sp.GetRequiredService<IOptions<RavenOptions>>().Value;
                return setup.GetDocumentStore(setup.BeforeInitializeDocStore);
            });
            return services.AddScoped(sp => sp.GetRequiredService<IDocumentStore>().OpenSession());
        }
    }
}
