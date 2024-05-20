namespace AAI.DAL.Mongo.Models
{
    public class twitter_account_config : BaseMongoDTO
    {
        public short type { get; set; }
        public string bear_token { get; set; }
        public string kdt { get; set; }
        public string auth_token { get; set; }
        public string csrf_token { get; set; }
        public string account { get; set; }
        public string user_name { get; set; }
    }
}
