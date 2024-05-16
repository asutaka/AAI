using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AAI.DAL.Mongo
{
    public static class UniqueIdentifier
    {
        public static string New => ObjectId.GenerateNewId().ToString();
    }
    public abstract class BaseMongoDTO
    {
        private string _id;
        //protected string CollectionName { get; set; }

        protected BaseMongoDTO()
        {
            _id = UniqueIdentifier.New;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("objectId")]
        public string ObjectId
        {
            get => _id;
            set => _id = string.IsNullOrEmpty(value) ? UniqueIdentifier.New : value;
        }
    }
    public interface IMongoRepositoryBase<T> where T : BaseMongoDTO
    {
        /// <summary>
        /// Sets a collection
        /// </summary>
        bool SetCollection(string collectionName);


        /// <summary>
        /// Get all entities in collection
        /// </summary>
        /// <returns>collection of entities</returns>
        Task<List<T>> GetAllAsync();


        /// <summary>
        /// Get async entity by identifier 
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        Task<T> GetByIdAsync(string id);

        IQueryable<T> Table { get; }


        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        T Update(T entity);

        /// <summary>
        /// Async Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Async Update one field entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<bool> UpdateOneFieldAsync(string fieldName, dynamic value, FilterDefinition<T> filter);
        Task InsertOneAsync(T entity);
    }


    public abstract class MongoRepositoryBase<T> : IMongoRepositoryBase<T> where T : BaseMongoDTO
    {
        #region Fields
        protected IMongoCollection<T> _collection;

        protected IMongoDatabase _database;
        protected ILogger<MongoRepositoryBase<T>> _logger;

        public IMongoCollection<T> Collection => _collection;

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table => _collection.AsQueryable(new AggregateOptions { AllowDiskUse = true });

        /// <summary>
        /// Sets a collection
        /// </summary>
        public virtual bool SetCollection(string collectionName)
        {
            _collection = _collection.Database.GetCollection<T>(collectionName);
            return true;
        }

        public virtual string GetCollectionName()
        {
            return typeof(T).Name;
            //return "answer";
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>        
        public MongoRepositoryBase(string url, string database, string collection)
        {
            //var client = new MongoClient(MongoDataSettingsManager.ConnectionString);

            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(url));
            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
            var client = new MongoClient(clientSettings);


            _database = client.GetDatabase(database);
            _collection = _database.GetCollection<T>(collection);
        }

        //public MongoRepositoryBase(IMongoDatabase database, ILogger<MongoRepositoryBase<T>> logger)
        //{
        //    _database = database;
        //    _collection = _database.GetCollection<T>(GetCollectionName());
        //    _logger = logger;
        //}

        /// <summary>
        /// Get all entities in collection
        /// </summary>
        /// <returns>collection of entities</returns>
        public virtual async Task<List<T>> GetAllAsync()
        {
            var enties = await _collection.AsQueryable().ToListAsync();

            return enties;
        }

        /// <summary>
        /// Get async entity by identifier 
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual Task<T> GetByIdAsync(string id)
        {
            return _collection.Find(e => e.ObjectId == id).FirstOrDefaultAsync();
        }

        public virtual T Update(T entity)
        {
            try
            {
                _collection.ReplaceOne(x => x.ObjectId == entity.ObjectId, entity, new ReplaceOptions() { IsUpsert = false });
            }
            catch (Exception ex)
            {
                _logger.LogError($"IMongoRepositoryBase.Update|REPOSITORY: {typeof(T).Name.ToUpper()}Repository|EXCEPTION| {ex.Message}");
            }
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                await _collection.ReplaceOneAsync(x => x.ObjectId == entity.ObjectId, entity, new ReplaceOptions() { IsUpsert = false });

            }catch(Exception ex)
            {
                _logger.LogError($"IMongoRepositoryBase.UpdateAsync|REPOSITORY: {typeof(T).Name.ToUpper()}Repository|EXCEPTION| {ex.Message}");
            }
            return entity;
        }

        public async Task<bool> UpdateOneFieldAsync(string fieldName, dynamic value, FilterDefinition<T> filter)
        {
            var result = false;
            try
            {
                var update = Builders<T>.Update.Set(fieldName, value);

                UpdateResult updateResult = await _collection.UpdateOneAsync(filter, update);
                result = updateResult.IsAcknowledged;
            }
            catch (Exception ex)
            {
                _logger.LogError($"IMongoRepositoryBase.UpdateOneFieldAsync|REPOSITORY: {typeof(T).Name.ToUpper()}Repository|EXCEPTION| {ex.Message}");
                result = false;
            }

            return result;
        }

        public async Task InsertOneAsync(T entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"IMongoRepositoryBase.InsertOneAsync|REPOSITORY: {typeof(T).Name.ToUpper()}Repository|EXCEPTION| {ex.Message}");
            }
        }

        #endregion
    }
}
