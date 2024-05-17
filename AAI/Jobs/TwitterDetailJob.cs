using AAI.DAL.Mongo;
using AAI.DAL.Mongo.Models;
using AAI.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace AAI.Jobs
{
    [DisallowConcurrentExecution]
    public class TwitterDetailJob : IJob
    {
        private readonly ILogger<TwitterDetailJob> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly ITwitterContentRepo _twitterRepo;
        private readonly ITwitterContentDetailIDRepo _twitterIdRepo;
        private readonly ITwitterContentDetailRepo _twitterDetailRepo;
        private const string _feature = "{\"rweb_tipjar_consumption_enabled\":true,\"responsive_web_graphql_exclude_directive_enabled\":true,\"verified_phone_label_enabled\":false,\"creator_subscriptions_tweet_preview_api_enabled\":true,\"responsive_web_graphql_timeline_navigation_enabled\":true,\"responsive_web_graphql_skip_user_profile_image_extensions_enabled\":false,\"communities_web_enable_tweet_community_results_fetch\":true,\"c9s_tweet_anatomy_moderator_badge_enabled\":true,\"articles_preview_enabled\":true,\"tweetypie_unmention_optimization_enabled\":true,\"responsive_web_edit_tweet_api_enabled\":true,\"graphql_is_translatable_rweb_tweet_is_translatable_enabled\":true,\"view_counts_everywhere_api_enabled\":true,\"longform_notetweets_consumption_enabled\":true,\"responsive_web_twitter_article_tweet_consumption_enabled\":true,\"tweet_awards_web_tipping_enabled\":false,\"creator_subscriptions_quote_tweet_preview_enabled\":false,\"freedom_of_speech_not_reach_fetch_enabled\":true,\"standardized_nudges_misinfo\":true,\"tweet_with_visibility_results_prefer_gql_limited_actions_policy_enabled\":true,\"tweet_with_visibility_results_prefer_gql_media_interstitial_enabled\":true,\"rweb_video_timestamps_enabled\":true,\"longform_notetweets_rich_text_read_enabled\":true,\"longform_notetweets_inline_media_enabled\":true,\"responsive_web_enhance_cards_enabled\":false}";
        private const string _fieldToggles = "{\"withArticleRichContentState\":true,\"withArticlePlainText\":false}";
        private const string _bearToken = "AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA";
        private const string _kdt = "gg49NbH789mFv0eNuiyFrU5jz3VjehVrvQbbXF5D";
        private const string _authToken = "f9e17ac2bfb70d337fc7102aecc5370d6309c47e";
        private const string _csrfToken = "3b4aeadd6a90d95280d56b652d7a2629edbbf2100c56a60be0fbcef999bf79d1262588b2821b2b3b5912874f4940e9848864a1460076633d5a592e443648792e0a7a045a709f465827a0f654326b476e";

        public TwitterDetailJob(IHttpClientFactory factory,
                        ILogger<TwitterDetailJob> logger,
                        ITwitterContentRepo twitterRepo,
                        ITwitterContentDetailRepo twitterDetailRepo,
                        ITwitterContentDetailIDRepo twitterIdRepo)
        {
            _logger = logger;
            _factory = factory;
            _twitterRepo = twitterRepo;
            _twitterIdRepo = twitterIdRepo;
            _twitterDetailRepo = twitterDetailRepo;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var lID = await _twitterIdRepo.GetAllAsync();//
            if (!lID.Any())
                return;

            foreach (var itemDetail in lID)
            {
                try
                {
                    var valueJson = JsonConvert.SerializeObject(new TwitterObjectValueModel
                    {
                        focalTweetId = itemDetail.detailId,
                        with_rux_injections = false,
                        includePromotedContent = true,
                        withCommunity = true,
                        withQuickPromoteEligibilityTweetFields = true,
                        withBirdwatchNotes = true,
                        withVoice = true,
                        withV2Timeline = true,
                    }); 
                    var url = $"https://x.com/i/api/graphql/zJvfJs3gSbrVhC0MKjt_OQ/TweetDetail?variables={UrlEncoder.Default.Encode(valueJson)}&features={UrlEncoder.Default.Encode(_feature)}&fieldToggles={UrlEncoder.Default.Encode(_fieldToggles)}";
                    using (var httpClient = _factory.CreateClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearToken);
                        httpClient.DefaultRequestHeaders.Add("x-csrf-token", _csrfToken);
                        httpClient.BaseAddress = new Uri(url);
                        httpClient.DefaultRequestHeaders
                              .Accept
                              .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Add("Cookie", $"kdt={_kdt}; auth_token={_authToken}; ct0={_csrfToken}; ");
                        var request = new HttpRequestMessage(HttpMethod.Get, "");
                        request.Content = new StringContent("", Encoding.UTF8, "application/json");

                        //httpClient.GenerateCurlInConsole(request);
                        var response = await httpClient.SendAsync(request);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var contents = await response.Content.ReadAsStringAsync();
                            var responseData = JsonConvert.DeserializeObject<TwitterModel>(contents);
                            ////save data 
                            var instructions = responseData?.data?.threaded_conversation_with_injections_v2?.instructions;
                            if (instructions?.Any() ?? false)
                            {
                                foreach (var itemIns in instructions)
                                {
                                    if (itemIns.entries?.Any() ?? false)
                                    {
                                        foreach (var itemEntry in itemIns.entries)
                                        {
                                            if (string.IsNullOrWhiteSpace(itemEntry.entryId)
                                                || itemEntry.content?.itemContent is null)
                                                continue;

                                            //check exists(check entryid)
                                            var entities = await _twitterDetailRepo.GetListById(new List<string> { itemEntry.entryId });
                                            if (entities.Any())
                                            {
                                                ////update
                                                //var filter = Builders<twitter_content>.Filter.In(x => x.entryId, new List<string> { itemEntry.entryId }); 
                                                //var isUpdate = await _twitterRepo.UpdateOneFieldAsync("content", itemEntry.content, filter);
                                                continue;
                                            }
                                            itemEntry.reply_to_thread = itemDetail.detailId;
                                            itemEntry.reply_to_kol = itemDetail.kolId;
                                            itemEntry.time = DateTime.Now.Ticks;
                                            itemEntry.completeCrawl = false;
                                            //insert database
                                            await _twitterDetailRepo.InsertOneAsync(itemEntry);
                                            //insert to twitter_content_id
                                            var detailId = itemEntry?.content?.itemContent?.tweet_results?.result?.legacy?.id_str;
                                            var timeString = itemEntry?.content?.itemContent?.tweet_results?.result?.legacy?.created_at;
                                            var userContent = itemEntry?.content?.itemContent?.tweet_results?.result?.core?.user_results?.result?.rest_id;
                                            var userItem = itemEntry?.content?.items.FirstOrDefault()?.item?.itemContent?.tweet_results?.result?.core?.user_results?.result?.rest_id;
                                            if (!string.IsNullOrWhiteSpace(detailId)
                                                && !string.IsNullOrWhiteSpace(timeString))
                                            {
                                                var kol = string.IsNullOrWhiteSpace(userContent) ? userItem : userContent;
                                                await _twitterIdRepo.InsertOneAsync(new twitter_detail_id
                                                {
                                                    detailId = detailId,
                                                    kolId = kol,
                                                    time = timeString.DateStringToLong("ddd MMM dd HH:mm:ss +0000 yyyy"),
                                                    isCrawl = false
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"TwitterDetailJob.Execute|EXCEPTION|{ex.Message}");
                }
                Thread.Sleep(500);
            }
        }

        #region SubClass
        private class TwitterModel
        {
            public TwitterDataModel data { get; set; }
        }
        private class TwitterDataModel
        {
            public TwitterDataInjectionModel threaded_conversation_with_injections_v2 { get; set; }
        }
        private class TwitterDataInjectionModel
        {
            public List<TwitterDataUserResulstTimeLineInstructionModel> instructions { get; set; }
        }
        private class TwitterDataUserResulstTimeLineInstructionModel
        {
            public List<twitter_detail> entries { get; set; }
        }

        private class TwitterObjectValueModel
        {
            public string focalTweetId { get; set; }
            //public int count { get; set; }
            public bool with_rux_injections { get; set; }
            public bool includePromotedContent { get; set; }
            public bool withCommunity { get; set; }
            public bool withQuickPromoteEligibilityTweetFields { get; set; }
            public bool withBirdwatchNotes { get; set; }
            public bool withVoice { get; set; }
            public bool withV2Timeline { get; set; }
        } 
        #endregion
    }
}
