using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Discoverer
{
    public class TwitterEngine
    {
        public List<Tweet> GetFavoriteList(string userName, int count, string source)
        {
            return GetTweetList(userName, count, source);
        }

        public List<User> GetFollowList(string userName, string followType)
        {
            string str = string.Format("http://m.twitter.com/{0}/{1}", userName, followType);
            string source = WebUtil.GetSource(str);
            List<User> users = ParseListOfUsers(source);
            return users;
        }

        public List<Tweet> GetTweetList(string userName, int count, string source)
        {
            List<Tweet> tweets = new List<Tweet>();
            foreach (Match match in Regex.Matches(source, "(?<=<table class=\"tweet).*?(?=<\\/table>)", RegexOptions.Singleline))
            {
                // TODO: Handle emoji characters
                Tweet tweet = new Tweet()
                {
                    AuthorUsername = Regex.Match(match.Value, "(?<=<span>@<\\/span>).*(?=\\s*<\\/div>)").Value,
                    AuthorName = Regex.Match(match.Value, "(?<=<strong class=\"fullname\">).*(?=<\\/strong>)").Value,
                    Content = StripTags(Regex.Match(match.Value, "(?<=<div class=\"dir-ltr\" dir=\"ltr\">).*(?=\\s*<\\/div>)").Value.Trim()),
                    Id = Regex.Match(match.Value, "(?<=<div class=\"tweet-text\" data-id=\")\\d+(?=\">)").Value,
                    //Date = Regex.Match(match.Value, "(?<=<time datetime=\")[\\s\\S]*(?=\">)").Value
                };
                tweets.Add(tweet);
            }
            return tweets;
        }

        private string StripTags(string text)
        {
            foreach (Match match in Regex.Matches(text, "<a[\\s\\S]*?>.*?<\\/a>"))
            {
                string value = Regex.Match(match.Value, "(?<=>).*(?=<\\/a>)").Value;
                text = text.Replace(match.Value, string.Format("[{0}]", value));
            }
            text = WebUtility.HtmlDecode(text);
            return text;
        }

        public User GetUser(string userName)
        {
            string str = string.Concat("https://m.twitter.com/", userName);
            string source = WebUtil.GetSource(str);
            string source1 = WebUtil.GetSource(string.Concat(str, "/favorites"));
            User user = new User()
            {
                IsProtected = source.Contains("<div class='protected'>"),
                FullName = Regex.Match(source, "(?<=<div class=\"fullname\">).*(?=\\s*<\\/div>)").Value,
                UserName = Regex.Match(source, "(?<=<span class=\"screen-name\">).*(?=\\s*<\\/span>)").Value,
                Location = Regex.Match(source, "(?<=<div class=\"location\">).*(?=\\s*<\\/div>)").Value,
                Bio = StripTags(Regex.Match(source, "(?<=<div class=\"bio\">\\s*<div class=\"dir-ltr\" dir=\"ltr\">\\s*).*(?=\\s*<\\/div>)").Value.Trim()),
                AvatarUrl = string.Concat(Regex.Match(source, "(?<=<td class=\"avatar\">\\s*<img alt=\".*?\" src=\")[\\s\\S]*?(?=_normal)").Value, ".jpg"),
                TweetCount = Regex.Match(source, "(?<=<div class=\"statnum\">)[0-9,KM]*(?=<\\/div>\\s*<div class=\"statlabel\"> Tweets <\\/div>)").Value,
                FollowingCount = Regex.Match(source, "(?<=<div class=\"statnum\">)[0-9,KM]*(?=<\\/div>\\s*<div class=\"statlabel\"> Following <\\/div>)").Value,
                FollowerCount = Regex.Match(source, "(?<=<div class=\"statnum\">)[0-9,KM]*(?=<\\/div>\\s*<div class=\"statlabel\"> Followers <\\/div>)").Value
            };
            if (!user.IsProtected)
            {
                user.TweetList = GetTweetList(userName, 0, source);
                user.FollowingList = GetFollowList(userName, "following");
                user.FollowerList = GetFollowList(userName, "followers");
                user.FavoriteList = GetFavoriteList(userName, 0, source1);
                user.FavoriteCount = Regex.Match(source1, "(?<=<span class=\"count\">)[0-9,KM]*(?=<\\/span>)").Value;
            }
            return user;
        }

        public List<User> SearchForUsers(string query)
        {
            string url = string.Format("https://m.twitter.com/search/users?q={0}", query);
            string source = WebUtil.GetSource(url);

            List<User> users = ParseListOfUsers(source);

            return users;
        }

        private List<User> ParseListOfUsers(string source)
        {
            List<User> users = new List<User>();

            foreach (Match match in Regex.Matches(source, "<td class=\"info fifty screenname\">[\\s\\S]*?<\\/td>"))
            {
                User user = new User()
                {
                    UserName = Regex.Match(match.Value, "(?<=<a name=\").*(?=\"\\/)").Value,
                    FullName = Regex.Match(match.Value, "(?<=<strong class=\"fullname\">).*(?=<\\/strong>)").Value
                };
                users.Add(user);
            }

            return users;
        }
    }
}