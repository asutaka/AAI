using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterContentDetailRepo : IMongoRepositoryBase<twitter_detail>
    {
        Task<List<twitter_detail>> GetListById(List<string> ids);
    }
    public class TwitterContentDetailRepo : MongoRepositoryBase<twitter_detail>, ITwitterContentDetailRepo
    {
        private readonly ILogger<TwitterContentDetailRepo> logger;

        public TwitterContentDetailRepo(ILogger<TwitterContentDetailRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterContentDetailCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_detail>> GetListById(List<string> ids)
        {
            var res = new List<twitter_detail>();

            try
            {
                var builder = Builders<twitter_detail>.Filter;
                var filter = builder.In(x => x.entryId, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterContentDetailRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }
    }
}
