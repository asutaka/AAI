using System.Collections.Generic;

namespace AAI.DAL.Mongo.Models
{
    public class twitter_post : BaseMongoDTO
    {
        public Twitter_PostInfo post_info { get; set; }
        public Twitter_UserInfo user_info { get; set; }
        public Twitter_PostDetail post_detail { get; set; }
    }
    public class twitter_post_detail : BaseMongoDTO
    {
        public Twitter_PostInfo post_info { get; set; }
        public Twitter_ParentInfo parent_info { get; set; }
        public twitter_user user_info { get; set; }
        public Twitter_PostDetail post_detail { get; set; }
    }

    public class twitter_user : BaseMongoDTO
    {
        public long create_at { get; set; }
        public string create_at_text { get; set; }
        public long favourites_count { get; set; }
        public long followers_count { get; set; }
        public long friends_count { get; set; }
        public string id { get; set; }
        public long listed_count { get; set; }
        public string location { get; set; }
        public long media_count { get; set; }
        public string name { get; set; }
        public long statuses_count { get; set; }
    }

    public class Twitter_PostInfo
    {
        public string id { get; set; }
        public long create_at { get; set; }
        public string create_at_text { get; set; }
        public long crawl_at { get; set; }
        public long crawl_update_at { get; set; }
        public bool complete_crawl { get; set; }//bien xac dinh ket thuc crawl post nay(36h sau khi post)
        public short post_type { get; set; }//0: tweet, 1: retweet, 2: quote
        public string post_url { get; set; }
    }
    public class Twitter_ParentInfo
    {
        public string origin_kol { get; set; }
        public string origin_post { get; set; }
        public string parent_user { get; set; }
        public string parent_post { get; set; }
    }
    public class Twitter_UserInfo 
    {
        public long create_at { get; set; }
        public string create_at_text { get; set; }
        public long favourites_count { get; set; }
        public long followers_count { get; set; }
        public long friends_count { get; set; }
        public string id { get; set; }
        public long listed_count { get; set; }
        public string location { get; set; }
        public long media_count { get; set; }
        public string name { get; set; }
        public long statuses_count { get; set; }
    }
    public class Twitter_PostDetail
    {
        public long bookmark_count { get; set; }
        public long favorite_count { get; set; }
        public string full_text { get; set; }
        public long quote_count { get; set; }
        public long reply_count { get; set; }
        public long retweet_count { get; set; }
        public long views { get; set; }
        public IEnumerable<Twitter_UserMentionInfo> user_mentions { get; set; }
        public List<TwitterHashtagModel> hashtags { get; set; }
        public TwitterQuoteModel quoted_status_result { get; set; }
        public TwitterRetweetModel retweeted_status_result { get; set; }
    }
    public class Twitter_UserMentionInfo
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
