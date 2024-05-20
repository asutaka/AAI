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
            services.AddScoped<ITwitterContentDetailRepo, TwitterContentDetailRepo>();
            services.AddScoped<ITwitterKolRepo, TwitterKolRepo>();

            services.AddScoped<ITwitterAccountConfigRepo, TwitterAccountConfigRepo>();
            services.AddScoped<ITwitterPostRepo, TwitterPostRepo>();
            services.AddScoped<ITwitterPostDetailRepo, TwitterPostDetailRepo>();
            services.AddScoped<ITwitterUserRepo, TwitterUserRepo>();

        }
    }
}
