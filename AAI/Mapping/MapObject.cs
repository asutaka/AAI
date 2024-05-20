using AAI.DAL.Mongo.Models;
using AAI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AAI.Mapping
{
    public static class MapObject
    {
        public static List<twitter_post> ToTwitterPost(this twitter_content model, string kolId)
        {
            var lResult = new List<twitter_post>();
            var lChild = model?.content?.items?.Select(x => x.item?.itemContent?.tweet_results?.result)?.ToList();
            if (lChild is null)
            {
                lChild = new List<TwitterEntryContentItemResultChildModel>();
            }
            var child = model?.content?.itemContent?.tweet_results?.result;
            if (child is not null)
            {
                lChild.Add(child);
            }
            foreach (var childModel in lChild)
            {
                var entity = new twitter_post
                {
                    parent_info = new Twitter_ParentInfo(),
                    post_info = new Twitter_PostInfo(),
                    post_detail = new Twitter_PostDetail(),
                    user_info = new Twitter_UserInfo()
                };
                try
                {
                    #region user_info
                    var userId = childModel.core?.user_results?.result?.rest_id;
                    var userChildModel = childModel.core?.user_results?.result?.legacy;
                    if (userId is null
                        || userChildModel is null)
                        continue;
                    entity.user_info.create_at = userChildModel.created_at.DateStringToLong("ddd MMM dd HH:mm:ss +0000 yyyy");
                    entity.user_info.create_at_text = userChildModel.created_at;
                    entity.user_info.favourites_count = userChildModel.favourites_count;
                    entity.user_info.followers_count = userChildModel.followers_count;
                    entity.user_info.friends_count = userChildModel.friends_count;
                    entity.user_info.id = userId;
                    entity.user_info.listed_count = userChildModel.listed_count;
                    entity.user_info.location = userChildModel.location;
                    entity.user_info.media_count = userChildModel.media_count;
                    entity.user_info.name = userChildModel.name;
                    entity.user_info.statuses_count = userChildModel.statuses_count;
                    #endregion

                    var postChildModel = childModel.legacy;
                    if (postChildModel is null)
                        continue;
                    //type
                    var type = ETweetPostType.Tweet;
                    if (childModel.quoted_status_result is not null)
                    {
                        type = ETweetPostType.Quote;
                    }
                    else if (postChildModel.retweeted_status_result is not null)
                    {
                        type = ETweetPostType.Retweet;
                    }
                    else if (!string.IsNullOrWhiteSpace(postChildModel.in_reply_to_user_id_str))
                    {
                        type = ETweetPostType.Reply;
                    }

                    #region parent_info
                    if(type == ETweetPostType.Reply)
                    {
                        entity.parent_info.parent_post = postChildModel.in_reply_to_status_id_str;
                        entity.parent_info.parent_user = postChildModel.in_reply_to_user_id_str;
                        entity.parent_info.parent_url = $"https://x.com/{postChildModel.in_reply_to_screen_name}/status/{postChildModel.in_reply_to_status_id_str}";
                        entity.parent_info.origin_post = postChildModel.in_reply_to_status_id_str;
                        entity.parent_info.origin_user = postChildModel.in_reply_to_user_id_str;
                        entity.parent_info.origin_url = $"https://x.com/{postChildModel.in_reply_to_screen_name}/status/{postChildModel.in_reply_to_status_id_str}";
                    }    
                    #endregion

                    #region post_info
                    entity.post_info.id = postChildModel.id_str;
                    entity.post_info.create_at = postChildModel.created_at.DateStringToLong("ddd MMM dd HH:mm:ss +0000 yyyy");
                    entity.post_info.create_at_text = postChildModel.created_at;
                    entity.post_info.crawl_at = DateTime.Now.Ticks;
                    entity.post_info.crawl_update_at = entity.post_info.crawl_at;
                    entity.post_info.complete_crawl = false;
                    entity.post_info.post_url = $"https://x.com/{userChildModel.screen_name}/status/{postChildModel.id_str}";
                    entity.post_info.post_type = (short)type; 
                    #endregion

                    #region post_detail
                    entity.post_detail.bookmark_count = postChildModel.bookmark_count;
                    entity.post_detail.favorite_count = postChildModel.favorite_count;
                    entity.post_detail.full_text = postChildModel.full_text;
                    entity.post_detail.quote_count = postChildModel.quote_count;
                    entity.post_detail.reply_count = postChildModel.reply_count;
                    entity.post_detail.retweet_count = postChildModel.retweet_count;
                    entity.post_detail.views = childModel.views.count;
                    entity.post_detail.user_mentions = postChildModel.entities?.user_mentions?.Select(x => new Twitter_UserMentionInfo
                    {
                        id = x.id_str,
                        name = x.name
                    }) ?? null;
                    entity.post_detail.hashtags = postChildModel.entities?.hashtags ?? null;
                    entity.post_detail.quoted_status_result = childModel.quoted_status_result;
                    entity.post_detail.retweeted_status_result = childModel.legacy.retweeted_status_result;
                    #endregion

                    lResult.Add(entity);
                }
                catch (Exception ex)
                {
                    NLogLogger.PublishException(ex, $"MapObject.ToTwitterPost|EXCEPTION| {ex.Message}");
                }
            }
            return lResult;
        }
    }
}

