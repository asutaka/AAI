using AAI.DAL.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace AAI.DAL
{
    public static class DALRepoDI
    {
        public static void DALDependency(this IServiceCollection services)
        {
            services.AddScoped<ITwitterContentRepo, TwitterContentRepo>();
            services.AddScoped<ITwitterContentDetailIDRepo, TwitterContentDetailIDRepo>();
        }
    }
}
