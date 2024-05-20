using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterAccountConfigRepo : IMongoRepositoryBase<twitter_account_config>
    {
        Task<List<twitter_account_config>> GetListById(List<string> ids);
    }
    public class TwitterAccountConfigRepo : MongoRepositoryBase<twitter_account_config>, ITwitterAccountConfigRepo
    {
        private readonly ILogger<TwitterAccountConfigRepo> logger;

        public TwitterAccountConfigRepo(ILogger<TwitterAccountConfigRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterAccountConfigCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_account_config>> GetListById(List<string> ids)
        {
            var res = new List<twitter_account_config>();

            try
            {
                var builder = Builders<twitter_account_config>.Filter;
                var filter = builder.In(x => x.ObjectId, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterAccountConfigRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }
    }
}
