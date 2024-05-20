using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterPostDetailRepo : IMongoRepositoryBase<twitter_post_detail>
    {
        Task<List<twitter_post_detail>> GetListById(List<string> ids);
    }
    public class TwitterPostDetailRepo : MongoRepositoryBase<twitter_post_detail>, ITwitterPostDetailRepo
    {
        private readonly ILogger<TwitterPostDetailRepo> logger;

        public TwitterPostDetailRepo(ILogger<TwitterPostDetailRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterPostDetailCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_post_detail>> GetListById(List<string> ids)
        {
            var res = new List<twitter_post_detail>();

            try
            {
                var builder = Builders<twitter_post_detail>.Filter;
                var filter = builder.In(x => x.ObjectId, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterPostDetailRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }
    }
}
