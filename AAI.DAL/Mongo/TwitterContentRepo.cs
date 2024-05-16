using AAI.DAL.Mongo.Models;
using AAI.DAL.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public interface ITwitterContentRepo : IMongoRepositoryBase<twitter_content>
    {
        Task<List<twitter_content>> GetListById(List<string> ids);
    }

    public class TwitterContentRepo : MongoRepositoryBase<twitter_content>, ITwitterContentRepo
    {

        private readonly ILogger<TwitterContentRepo> logger;

        public TwitterContentRepo(ILogger<TwitterContentRepo> logger) : base(MongoConfig.ConnectionString,
                                                                                        MongoConfig.DatabaseName,
                                                                                        MongoConfig.TwitterContentCollection)
        {
            this.logger = logger;
        }

        public async Task<List<twitter_content>> GetListById(List<string> ids)
        {
            var anwser = new List<twitter_content>();

            try
            {
                var builder = Builders<twitter_content>.Filter;
                var filter = builder.In(x => x.entryId, ids);
                anwser = await _collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"TwitterContentRepo.GetListById|EXCEPTION| {ex.Message}");
            }

            return anwser;
        }
    }
}
