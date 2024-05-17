using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterKolRepo : IMongoRepositoryBase<twitter_kol>
    {
        Task<List<twitter_kol>> GetListById(List<string> ids);
    }
    public class TwitterKolRepo : MongoRepositoryBase<twitter_kol>, ITwitterKolRepo
    {
        private readonly ILogger<TwitterKolRepo> logger;

        public TwitterKolRepo(ILogger<TwitterKolRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterKolCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_kol>> GetListById(List<string> ids)
        {
            var res = new List<twitter_kol>();

            try
            {
                var builder = Builders<twitter_kol>.Filter;
                var filter = builder.In(x => x.ObjectId, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterKolRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }
    }
}
