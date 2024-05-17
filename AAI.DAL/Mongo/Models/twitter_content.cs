using System.Collections.Generic;

namespace AAI.DAL.Mongo.Models
{
    public class twitter_content : BaseMongoDTO
    {
        //xđ bản ghi thuộc KOL nào
        public string kolId { get; set; }
        public long time { get; set; }
        public bool completeCrawl { get; set; }//Khi bài tweet đã đăng đủ lâu(36 tiếng) thì dừng crawl các phản hồi bên trong
        public string entryId { get; set; }
        public string sortIndex { get; set; }
        public TwitterEntryContentModel content { get; set; }
    }
    public class TwitterEntryContentModel
    {
        public TwitterEntryContentItemModel itemContent { get; set; }
    }
    public class TwitterEntryContentItemModel
    {
        public TwitterEntryContentItemResultModel tweet_results { get; set; }
    }
    public class TwitterEntryContentItemResultModel
    {
        public TwitterEntryContentItemResultChildModel result { get; set; }
    }
    public class TwitterEntryContentItemResultChildModel
    {
        public string rest_id { get; set; }
        //core
        public TwitterEntryContentItemResultCoreModel core { get; set; }
        //unmention_data
        //edit_control
        //views
        public TwitterEntryContentItemResultChildViewModel views { get; set; }
        //source
        //legacy
        public TwitterEntryContentItemResultChildLegacyModel legacy { get; set; }
    }

    public class TwitterEntryContentItemResultCoreModel
    {
        public TwitterEntryContentItemResultUserModel user_results { get; set; }
    }

    public class TwitterEntryContentItemResultUserModel
    {
        public TwitterEntryContentItemResultUserResultModel result { get; set; }
    }

    public class TwitterEntryContentItemResultUserResultModel
    {
        public string rest_id { get; set; }
        public TwitterEntryContentItemResultUserLegacyModel legacy { get; set; }
    }

    public class TwitterEntryContentItemResultUserLegacyModel
    {
        public string created_at { get; set; }
        public long favourites_count { get; set; }
        public long followers_count { get; set; }
        public long friends_count { get; set; }
        public long listed_count { get; set; }
        public string location { get; set; }
        public long media_count { get; set; }
        public string name { get; set; }
        public long normal_followers_count { get; set; }
        public long statuses_count { get; set; }
    }


    public class TwitterEntryContentItemResultChildViewModel
    {
        public long count { get; set; }
    }
    public class TwitterEntryContentItemResultChildLegacyModel
    {
        public long bookmark_count { get; set; }
        public string created_at { get; set; }
        //entities
        public TwitterEntryContentItemResultChildLegacyEntityModel entities { get; set; }
        //extened_entities
        public long favorite_count { get; set; }
        public string full_text { get; set; }
        public long quote_count { get; set; }
        public long reply_count { get; set; }
        public long retweet_count { get; set; }
        public string id_str { get; set; }
    }
    public class TwitterEntryContentItemResultChildLegacyEntityModel
    {
        public List<TwitterHashtagModel> hashtags { get; set; }
        public List<TwitterMediaModel> media { get; set; }
        public List<TwitterUserMention> user_mentions { get; set; }
        public List<TwitterUrl> urls { get; set; }
    }
    public class TwitterHashtagModel
    {
        public string text { get; set; }
    }
    public class TwitterMediaModel
    {
        public string expanded_url { get; set; }
    }

    public class TwitterUserMention
    {
        public string id_str { get; set; }
        public string name { get; set; }
    }

    public class TwitterUrl
    {
        public string url { get; set; }
    }
}
