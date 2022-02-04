namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        //Hack to make EF Core scafolding work. For some reason, this method is not accessible in the notebook
        public static ServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection)
        {
            return ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(serviceCollection);
        }
    }
}
