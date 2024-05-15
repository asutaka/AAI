using Quartz;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using HttpClientToCurl;

namespace AAI.Jobs
{
    [DisallowConcurrentExecution]
    public class TwitterJob : IJob
    {
        private readonly IHttpClientFactory _factory;
        private const string _feature = "{ \"rweb_tipjar_consumption_enabled\": true,\"responsive_web_graphql_exclude_directive_enabled\": true,\"verified_phone_label_enabled\": false,\"creator_subscriptions_tweet_preview_api_enabled\": true,\"responsive_web_graphql_timeline_navigation_enabled\": true,\"responsive_web_graphql_skip_user_profile_image_extensions_enabled\": false,\"communities_web_enable_tweet_community_results_fetch\": true,\"c9s_tweet_anatomy_moderator_badge_enabled\": true,\"articles_preview_enabled\": true,\"tweetypie_unmention_optimization_enabled\": true,\"responsive_web_edit_tweet_api_enabled\": true,\"graphql_is_translatable_rweb_tweet_is_translatable_enabled\": true,\"view_counts_everywhere_api_enabled\": true,\"longform_notetweets_consumption_enabled\": true,\"responsive_web_twitter_article_tweet_consumption_enabled\": true,\"tweet_awards_web_tipping_enabled\": false,\"creator_subscriptions_quote_tweet_preview_enabled\": false,\"freedom_of_speech_not_reach_fetch_enabled\": true,\"standardized_nudges_misinfo\": true,\"tweet_with_visibility_results_prefer_gql_limited_actions_policy_enabled\": true,\"tweet_with_visibility_results_prefer_gql_media_interstitial_enabled\": true,\"rweb_video_timestamps_enabled\": true,\"longform_notetweets_rich_text_read_enabled\": true,\"longform_notetweets_inline_media_enabled\": true,\"responsive_web_enhance_cards_enabled\": false }";
        private const string _fieldToggles = "{ \"withArticlePlainText\": false }";
        private const string _userID = "330262748";
        private const string _bearToken = "AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA";
        private const string _kdt = "gg49NbH789mFv0eNuiyFrU5jz3VjehVrvQbbXF5D";
        private const string _authToken = "f9e17ac2bfb70d337fc7102aecc5370d6309c47e";
        private const string _csrfToken = "3b4aeadd6a90d95280d56b652d7a2629edbbf2100c56a60be0fbcef999bf79d1262588b2821b2b3b5912874f4940e9848864a1460076633d5a592e443648792e0a7a045a709f465827a0f654326b476e";

        public TwitterJob(IHttpClientFactory factory) 
        {
            _factory = factory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var valueJson = JsonConvert.SerializeObject(new TwitterObjectValueModel
            {
                userId = _userID,
                count = 20,
                includePromotedContent = true,
                withQuickPromoteEligibilityTweetFields = true,
                withVoice = true,
                withV2Timeline = true,
            });
            var url = $"https://twitter.com/i/api/graphql/9zyyd1hebl7oNWIPdA8HRw/UserTweets?variables={UrlEncoder.Default.Encode(valueJson)}&features={UrlEncoder.Default.Encode(_feature)}&fieldToggles={UrlEncoder.Default.Encode(_fieldToggles)}";
            try
            {
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

                    httpClient.GenerateCurlInConsole(request);

                    var response = await httpClient.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var contents = await response.Content.ReadAsStringAsync();
                        var tmp = 1;
                        //var responseData = JsonConvert.DeserializeObject<GetQuestion_Response>(contents);

                        //return responseData;

                    }
                }
            }
            catch (Exception ex)
            {
                var tmp = 1;
            }
        }
    }

    public class TwitterObjectValueModel
    {
        public string userId { get; set; }
        public int count { get; set; }
        public bool includePromotedContent { get; set; }
        public bool withQuickPromoteEligibilityTweetFields { get; set; }
        public bool withVoice { get; set; }
        public bool withV2Timeline { get; set; }
    }
}
