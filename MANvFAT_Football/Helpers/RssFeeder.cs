using MANvFAT_Football.Models.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MANvFAT_Football.Helpers
{
    public class RssFeeder
    {
        public static List<LatestPostFeeds> GetLatestPostFeeds()
        {
            string jsonRSS = "";
            using (WebClient wc = new WebClient())
            {
                jsonRSS = wc.DownloadString("https://talk.manvfat.com/c/sweating/mvf-football.json");
            }

            List<LatestPostFeeds> feeds = new List<LatestPostFeeds>();

            var model = JsonConvert.DeserializeObject<RootObject>(jsonRSS);

            foreach (var item in model.topic_list.topics)
            {
                var user = (from u in model.users
                            join p in item.posters on u.id equals p.user_id
                            select new User
                            {
                                id = u.id,
                                username = u.username.Replace("_", " "),
                                avatar_template = "https://talk.manvfat.com" + u.avatar_template.Replace("{size}", "100")
                            }).ToList();

                LatestPostFeeds feed = new LatestPostFeeds()
                {
                    Title = item.title,
                    ShortDescription = item.excerpt,
                    Link = "https://talk.manvfat.com" + "/t/" + item.slug + "/" + item.id,
                    Tag = item.tags != null ? item.tags.FirstOrDefault() : null,
                    TagLink = item.tags != null ? "https://talk.manvfat.com/tags/" + item.tags.FirstOrDefault() : null,
                    users = user
                };

                feeds.Add(feed);
            }

            return feeds.Take(10).ToList();
        }

        public static List<LatestPostFeeds> GetLeagueNewsFeed(Controller ctrl)
        {
            List<LatestPostFeeds> Final_Feeds = new List<LatestPostFeeds>();

            List<LatestPostFeeds> Global_Feeds = new List<LatestPostFeeds>();
            List<LatestPostFeeds> League_Feeds = new List<LatestPostFeeds>();

            //if (ctrl.Session["Fixture_LeagueID"] != null)
            //{
            //    long LeagueID = Convert.ToInt64(ctrl.Session["Fixture_LeagueID"]);
            //    LeaguesRepository leagueRepo = new LeaguesRepository();
            //    var league = leagueRepo.ReadOne(LeagueID);
            //    if (!string.IsNullOrEmpty(league.NewsTag))
            //    {
            //        string jsonRSS = "";
            //        try
            //        {
            //            using (WebClient wc = new WebClient())
            //            {
            //                jsonRSS = wc.DownloadString("https://talk.manvfat.com/tags/" + league.NewsTag + ".json");
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            //404 Not Found error Occurred, when a tag is entered in Leagues maintenance screen but it is not available on https://talk.manvfat.com/tags/ server
            //            SecurityUtils.AddAuditLog("RSS Feed", "RSS Feed 404 Not Found error Occurred for League: " + league.LeagueName + " NewsTag: " + league.NewsTag + " Not Available");
            //            // ErrorHandling.HandleException(ex);
            //            //  return League_Feeds;
            //        }

            //        if (!string.IsNullOrEmpty(jsonRSS))
            //        {
            //            var model = JsonConvert.DeserializeObject<RootObject>(jsonRSS);

            //            foreach (var item in model.topic_list.topics)
            //            {
            //                var user = (from u in model.users
            //                            join p in item.posters on u.id equals p.user_id
            //                            select new User
            //                            {
            //                                id = u.id,
            //                                username = u.username.Replace("_", " "),
            //                                avatar_template = "https://talk.manvfat.com" + u.avatar_template.Replace("{size}", "100")
            //                            }).ToList();

            //                LatestPostFeeds feed = new LatestPostFeeds()
            //                {
            //                    Title = SecurityUtils.TrimText(item.title,62),
            //                    PublishDate = item.created_at,
            //                    LastUpdatedDate = item.bumped_at,
            //                    ShortDescription = item.excerpt,
            //                    Link = "https://talk.manvfat.com" + "/t/" + item.slug + "/" + item.id,
            //                    Tag = item.tags.FirstOrDefault(),
            //                    TagLink = "https://talk.manvfat.com/tags/" + item.tags.FirstOrDefault(),
            //                    users = user
            //                };

            //                feed.Title = SetMinimumLengthOfTitle(feed.Title);

            //                League_Feeds.Add(feed);
            //            }
            //        }
            //    }
            //}

            //Global Feeds
            {
                string jsonRSS = "";
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        jsonRSS = wc.DownloadString("https://talk.manvfat.com/tags/global.json");
                    }
                }
                catch (Exception ex)
                {
                    //404 Not Found error Occurred, when a tag is entered in Leagues maintenance screen but it is not available on https://talk.manvfat.com/tags/ server
                    SecurityUtils.AddAuditLog("RSS Feed", "RSS Feed 404 Not Found error Occurred for Global Feeds Not Available");
                    // ErrorHandling.HandleException(ex);
                    // return League_Feeds;
                }

                if (!string.IsNullOrEmpty(jsonRSS))
                {
                    var model = JsonConvert.DeserializeObject<RootObject>(jsonRSS);

                    foreach (var item in model.topic_list.topics)
                    {
                        var user = (from u in model.users
                                    join p in item.posters on u.id equals p.user_id
                                    select new User
                                    {
                                        id = u.id,
                                        username = u.username.Replace("_", " "),
                                        avatar_template = "https://talk.manvfat.com" + u.avatar_template.Replace("{size}", "100")
                                    }).ToList();

                        LatestPostFeeds feed = new LatestPostFeeds()
                        {
                            Title =  SecurityUtils.TrimText(item.title, 62),
                            PublishDate = item.created_at,
                            LastUpdatedDate = item.bumped_at,
                            ShortDescription = item.excerpt,
                            Link = "https://talk.manvfat.com" + "/t/" + item.slug + "/" + item.id,
                            Tag = item.tags.FirstOrDefault(),
                            TagLink = "https://talk.manvfat.com/tags/" + item.tags.FirstOrDefault(),
                            users = user
                        };

                        feed.Title = SetMinimumLengthOfTitle(feed.Title);

                        Global_Feeds.Add(feed);
                    }
                }
            }

            //Now Fill the Final Feeds
            //We Need to Display 8 feeds in total, if we have 2 Global then we need to pick only 6 from League Feeds to make it 8 feeds in total
            {
                int TotalFinalFeedsRequired = 8;

                //Take onlly 8 Global Feeds
                Global_Feeds = Global_Feeds.OrderByDescending(m=>m.LastUpdatedDate).Take(4).ToList();
                
                var TotalNumOfGlobal_Feeds = Global_Feeds.Count();


                //Take only Feeds minus Global League
                League_Feeds = League_Feeds.OrderByDescending(m=>m.LastUpdatedDate).Take(TotalFinalFeedsRequired - TotalNumOfGlobal_Feeds).ToList();

                Final_Feeds.AddRange(Global_Feeds);

                Final_Feeds.AddRange(League_Feeds);
            }


            return Final_Feeds.OrderByDescending(m=>m.LastUpdatedDate).ToList();
        }

        public static string SetMinimumLengthOfTitle(string Title)
        {
            int MinLength = 38;

            if (Title.Length < MinLength)
            {
                var NumOfCharactersReqruied = MinLength - Title.Length;
                for (int i = 0; i < NumOfCharactersReqruied; i++)
                {
                    Title = Title + "&nbsp;";
                }
            }

            return Title;
        }

        /// <summary>
        ///   2 latest posts tagged from that location (e.g. "Manchester")  https://talk.manvfat.com/tags/manchester.json
        ///   2 latest posts tagged from "global"
        ///   1 hottest topic posts https://talk.manvfat.com/top/weekly.json
        /// </summary>
        /// <param name="PlayerID"></param>
        /// <returns></returns>
        public static List<LatestPostFeeds> GetProgresDashboardFeeds(long PlayerID)
        {
            List<LatestPostFeeds> Final_Feeds = new List<LatestPostFeeds>();

            List<LatestPostFeeds> Global_Feeds = new List<LatestPostFeeds>();
            List<LatestPostFeeds> League_Feeds = new List<LatestPostFeeds>();
            List<LatestPostFeeds> Weekly_Feeds = new List<LatestPostFeeds>();

           

            //GEt Player League

            #region League News Tag Posts
            //LeaguesRepository leagueRepo = new LeaguesRepository();
            //var SelectedLeague = leagueRepo.GetPlayerSelectedLeague(PlayerID);

            //if (SelectedLeague.LeagueID.HasValue)
            //{
            //    long LeagueID = SelectedLeague.LeagueID.Value;
          
            //    var league = leagueRepo.ReadOne(LeagueID);
            //    if (!string.IsNullOrEmpty(league.NewsTag))
            //    {
            //        string jsonRSS = "";
            //        try
            //        {
            //            using (WebClient wc = new WebClient())
            //            {
            //                jsonRSS = wc.DownloadString("https://talk.manvfat.com/tags/" + league.NewsTag + ".json");
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            //404 Not Found error Occurred, when a tag is entered in Leagues maintenance screen but it is not available on https://talk.manvfat.com/tags/ server
            //            SecurityUtils.AddAuditLog("RSS Feed", "RSS Feed 404 Not Found error Occurred for League: " + league.LeagueName + " NewsTag: " + league.NewsTag + " Not Available");
            //            // ErrorHandling.HandleException(ex);
            //            //  return League_Feeds;
            //        }

            //        if (!string.IsNullOrEmpty(jsonRSS))
            //        {
            //            var model = JsonConvert.DeserializeObject<RootObject>(jsonRSS);

            //            foreach (var item in model.topic_list.topics)
            //            {
            //                var user = (from u in model.users
            //                            join p in item.posters on u.id equals p.user_id
            //                            select new User
            //                            {
            //                                id = u.id,
            //                                username = u.username.Replace("_", " "),
            //                                avatar_template = "https://talk.manvfat.com" + u.avatar_template.Replace("{size}", "100")
            //                            }).ToList();

            //                LatestPostFeeds feed = new LatestPostFeeds()
            //                {
            //                    Title = item.title,
            //                    PublishDate = item.created_at,
            //                    LastUpdatedDate = item.bumped_at,
            //                    ShortDescription = item.excerpt,
            //                    Link = "https://talk.manvfat.com" + "/t/" + item.slug + "/" + item.id,
            //                    Tag = item.tags.FirstOrDefault(),
            //                    TagLink = "https://talk.manvfat.com/tags/" + item.tags.FirstOrDefault(),
            //                    users = user
            //                };

            //               // feed.Title = SetMinimumLengthOfTitle(feed.Title);

            //                League_Feeds.Add(feed);
            //            }
            //        }
            //    }
            //}
            #endregion

            #region Global Posts
            {
                string jsonRSS = "";
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        jsonRSS = wc.DownloadString("https://talk.manvfat.com/tags/global.json");
                    }
                }
                catch (Exception ex)
                {
                    //404 Not Found error Occurred, when a tag is entered in Leagues maintenance screen but it is not available on https://talk.manvfat.com/tags/ server
                    SecurityUtils.AddAuditLog("RSS Feed", "RSS Feed 404 Not Found error Occurred for Global Feeds Not Available");
                    // ErrorHandling.HandleException(ex);
                    // return League_Feeds;
                }

                if (!string.IsNullOrEmpty(jsonRSS))
                {
                    var model = JsonConvert.DeserializeObject<RootObject>(jsonRSS);

                    foreach (var item in model.topic_list.topics)
                    {
                        var user = (from u in model.users
                                    join p in item.posters on u.id equals p.user_id
                                    select new User
                                    {
                                        id = u.id,
                                        username = u.username.Replace("_", " "),
                                        avatar_template = "https://talk.manvfat.com" + u.avatar_template.Replace("{size}", "100")
                                    }).ToList();

                        LatestPostFeeds feed = new LatestPostFeeds()
                        {
                            Title = item.title,
                            PublishDate = item.created_at,
                            LastUpdatedDate = item.bumped_at,
                            ShortDescription = item.excerpt,
                            Link = "https://talk.manvfat.com" + "/t/" + item.slug + "/" + item.id,
                            Tag = item.tags.FirstOrDefault(),
                            TagLink = "https://talk.manvfat.com/tags/" + item.tags.FirstOrDefault(),
                            users = user
                        };

                        //feed.Title = SetMinimumLengthOfTitle(feed.Title);

                        Global_Feeds.Add(feed);
                    }
                }
            }

            #endregion

            #region Weekly Posts
            {
                string jsonRSS = "";
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        jsonRSS = wc.DownloadString("https://talk.manvfat.com/top/weekly.json");
                    }
                }
                catch (Exception ex)
                {
                    //404 Not Found error Occurred, when a tag is entered in Leagues maintenance screen but it is not available on https://talk.manvfat.com/tags/ server
                    SecurityUtils.AddAuditLog("RSS Feed", "RSS Feed 404 Not Found error Occurred for Global Feeds Not Available");
                    // ErrorHandling.HandleException(ex);
                    // return League_Feeds;
                }

                if (!string.IsNullOrEmpty(jsonRSS))
                {
                    var model = JsonConvert.DeserializeObject<RootObject>(jsonRSS);

                    foreach (var item in model.topic_list.topics)
                    {
                        var user = (from u in model.users
                                    join p in item.posters on u.id equals p.user_id
                                    select new User
                                    {
                                        id = u.id,
                                        username = u.username.Replace("_", " "),
                                        avatar_template = "https://talk.manvfat.com" + u.avatar_template.Replace("{size}", "100")
                                    }).ToList();

                        LatestPostFeeds feed = new LatestPostFeeds()
                        {
                            Title = item.title,
                            PublishDate = item.created_at,
                            LastUpdatedDate = item.bumped_at,
                            ShortDescription = item.excerpt,
                            Link = "https://talk.manvfat.com" + "/t/" + item.slug + "/" + item.id,
                            Tag = item.tags.FirstOrDefault(),
                            TagLink = "https://talk.manvfat.com/tags/" + item.tags.FirstOrDefault(),
                            users = user
                        };

                       // feed.Title = SetMinimumLengthOfTitle(feed.Title);

                        Weekly_Feeds.Add(feed);
                    }
                }
            }

            #endregion


            //Get 2 latest posts tagged from that location (e.g. "Manchester")  https://talk.manvfat.com/tags/manchester.json
            League_Feeds = League_Feeds.OrderByDescending(m => m.LastUpdatedDate).Take(2).ToList();
            Global_Feeds = Global_Feeds.OrderByDescending(m => m.LastUpdatedDate).Take(2).ToList();
            Weekly_Feeds = Weekly_Feeds.OrderByDescending(m => m.LastUpdatedDate).Take(1).ToList();

            Final_Feeds.AddRange(League_Feeds);
            Final_Feeds.AddRange(Global_Feeds);
            Final_Feeds.AddRange(Weekly_Feeds);
            
            return Final_Feeds.OrderByDescending(m => m.LastUpdatedDate).ToList();
        }
    }

    public class RootObject
    {
        public User[] users { get; set; }
        public Topic_List topic_list { get; set; }
    }

    public class Topic_List
    {
        public bool can_create_topic { get; set; }
        public object draft { get; set; }
        public string draft_key { get; set; }
        public object draft_sequence { get; set; }
        public int per_page { get; set; }
        public Topic[] topics { get; set; }
    }

    public class Topic
    {
        public int id { get; set; }
        public string title { get; set; }
        public string fancy_title { get; set; }
        public string slug { get; set; }
        public int posts_count { get; set; }
        public int reply_count { get; set; }
        public int highest_post_number { get; set; }
        public string image_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? last_posted_at { get; set; }
        public bool bumped { get; set; }
        public DateTime bumped_at { get; set; }
        public bool unseen { get; set; }
        public bool pinned { get; set; }
        public object unpinned { get; set; }
        public string excerpt { get; set; }
        public bool visible { get; set; }
        public bool closed { get; set; }
        public bool archived { get; set; }
        public object bookmarked { get; set; }
        public object liked { get; set; }
        public int views { get; set; }
        public int like_count { get; set; }
        public bool has_summary { get; set; }
        public string archetype { get; set; }
        public string last_poster_username { get; set; }
        public int category_id { get; set; }
        public bool pinned_globally { get; set; }
        public string[] tags { get; set; }
        public Poster[] posters { get; set; }
    }

    public class Poster
    {
        public string extras { get; set; }
        public string description { get; set; }
        public int user_id { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string avatar_template { get; set; }
    }

    public class LatestPostFeeds
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public List<User> users { get; set; }

        //public string Category { get; set; }
        public string Link { get; set; }

        public string Tag { get; set; }
        public string TagLink { get; set; }

        public DateTime PublishDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        //public string Description { get; set; }
    }

    //////////////////AFTER THIS LINE///////////////
}