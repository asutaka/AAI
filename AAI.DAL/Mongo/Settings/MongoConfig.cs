using Microsoft.Extensions.Configuration;

namespace AAI.DAL.Settings
{
    public static class MongoConfig
    {
        public static IConfiguration configuration { get; set; }
        public static string ConnectionString { get { return configuration.GetSection("MongoDB:Connection").Value; } }
        public static string DatabaseName { get { return configuration.GetSection("MongoDB:Database").Value; } }
        public static string TwitterContentCollection { get { return configuration.GetSection("MongoDB:TwitterContentCollection").Value; } }
        public static string TwitterContentDetailCollection { get { return configuration.GetSection("MongoDB:TwitterContentDetailCollection").Value; } }
        public static string TwitterContentDetailIDCollection { get { return configuration.GetSection("MongoDB:TwitterContentDetailIDCollection").Value; } }
        public static string TwitterKolCollection { get { return configuration.GetSection("MongoDB:TwitterKolCollection").Value; } }
    }
}
