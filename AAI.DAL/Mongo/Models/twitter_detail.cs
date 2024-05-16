using System;
using System.Collections.Generic;
using System.Text;

namespace AAI.DAL.Mongo.Models
{
    public class twitter_detail : BaseMongoDTO
    {
        public string detailId { get; set; }
        public string entryId { get; set; }
        public string sortIndex { get; set; }
        public TwitterDetailEntryContentModel content { get; set; }
    }
    public class TwitterDetailEntryContentModel
    {
        public TwitterEntryContentItemModel itemContent { get; set; }
    }
}
