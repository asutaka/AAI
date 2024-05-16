using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterContentDetailIDRepo : IMongoRepositoryBase<twitter_detail_id>
    {
        Task<List<twitter_detail_id>> GetListById(List<string> ids);
        Task<List<twitter_detail_id>> GetAllWithOptionsAsync(int offset, int limit, FilterDefinition<twitter_detail_id> filter);
    }
    public class TwitterContentDetailIDRepo : MongoRepositoryBase<twitter_detail_id>, ITwitterContentDetailIDRepo
    {
        private readonly ILogger<TwitterContentDetailIDRepo> logger;

        public TwitterContentDetailIDRepo(ILogger<TwitterContentDetailIDRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterContentDetailIDCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_detail_id>> GetListById(List<string> ids)
        {
            var res = new List<twitter_detail_id>();

            try
            {
                var builder = Builders<twitter_detail_id>.Filter;
                var filter = builder.In(x => x.detailId, ids);
                res = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterContentDetailIDRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return res;
        }

        public virtual async Task<List<twitter_detail_id>> GetAllWithOptionsAsync(int offset, int limit, FilterDefinition<twitter_detail_id> filter)
        {
            var response = new List<twitter_detail_id>();

            try
            {
                response = await _collection
                        .Find(filter)
                        .Skip((offset - 1) * limit)
                        .Limit(limit)
                        .SortByDescending(x => x.time)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterContentDetailIDRepo.GetAllWithOptionsAsync|EXCEPTION| {ex.Message}");
            }

            return response;
        }

    }
}
