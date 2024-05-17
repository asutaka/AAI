namespace AAI.DAL.Mongo.Models
{
    public class twitter_detail_id : BaseMongoDTO
    {
        public string detailId { get; set; }
        public string kolId { get; set; }
        public long time { get; set; }
        public bool isCrawl { get; set; }
    }
}
