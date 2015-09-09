using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Discoverer
{
    public class User
    {
        public string UserName
        {
            get;
            set;
        }

        public string AvatarUrl
        {
            get;
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public bool IsProtected
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public string Bio
        {
            get;
            set;
        }

        public string TweetCount
        {
            get;
            set;
        }

        public string FollowingCount
        {
            get;
            set;
        }

        public string FollowerCount
        {
            get;
            set;
        }

        public string FavoriteCount
        {
            get;
            set;
        }


        public List<Tweet> TweetList
        {
            get;
            set;
        }

        public List<Tweet> FavoriteList
        {
            get;
            set;
        }

        public List<User> FollowerList
        {
            get;
            set;
        }

        public List<User> FollowingList
        {
            get;
            set;
        }
    }
}