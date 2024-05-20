using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterUserRepo : IMongoRepositoryBase<twitter_user>
    {
        Task<List<twitter_user>> GetListById(List<string> ids);
    }
    public class TwitterUserRepo : MongoRepositoryBase<twitter_user>, ITwitterUserRepo
    {
        private readonly ILogger<TwitterUserRepo> logger;

        public TwitterUserRepo(ILogger<TwitterUserRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterUserCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_user>> GetListById(List<string> ids)
        {
            var res = new List<twitter_user>();

            try
            {
                var builder = Builders<twitter_user>.Filter;
                var filter = builder.In(x => x.ObjectId, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterUserRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }
    }
}
