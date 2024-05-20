using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterPostRepo : IMongoRepositoryBase<twitter_post>
    {
        Task<List<twitter_post>> GetListById(List<string> ids);
    }
    public class TwitterPostRepo : MongoRepositoryBase<twitter_post>, ITwitterPostRepo
    {
        private readonly ILogger<TwitterPostRepo> logger;

        public TwitterPostRepo(ILogger<TwitterPostRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterPostCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_post>> GetListById(List<string> ids)
        {
            var res = new List<twitter_post>();

            try
            {
                var builder = Builders<twitter_post>.Filter;
                var filter = builder.In(x => x.post_info.id, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterPostRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }
    }
}
