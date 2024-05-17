using System.Collections.Generic;
namespace AAI.DAL.Mongo.Models
{
    public class twitter_detail : BaseMongoDTO
    {
        public string reply_to_thread { get; set; }//reply bài nào
        public string reply_to_kol { get; set; }//reply bài của KOL nào
        public long time { get; set; }
        public bool completeCrawl { get; set; }//Khi bài tweet đã đăng đủ lâu(36 tiếng) thì dừng crawl các phản hồi bên trong

        public string entryId { get; set; }
        public string sortIndex { get; set; }
        public TwitterDetailEntryContentModel content { get; set; }
    }
    public class TwitterDetailEntryContentModel
    {
        public TwitterEntryContentItemModel itemContent { get; set; }
        public List<TwitterEntryContentItemSubModel> items { get; set; }
    }

    public class TwitterEntryContentItemSubModel
    {
        public string entryId { get; set; }
        public TwitterEntryContentItemSubChildModel item { get; set; }
    }
    public class TwitterEntryContentItemSubChildModel
    {
        public TwitterEntryContentItemModel itemContent { get; set; }
    }
}
