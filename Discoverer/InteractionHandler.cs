using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace Discoverer
{
    public class InteractionHandler
    {
        TwitterEngine twitterEngine = new TwitterEngine();
        Dictionary<string, string> paths = new Dictionary<string, string>
        {
            {"archives", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discoverer\\Users"},
            {"home", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discoverer"}
        };

        public InteractionHandler()
        {
        }

        public void Start()
        {
            Console.WriteLine("Discoverer 1.0.0\n");
            bool exit = false;

            while (!exit)
            {
                ShowPrompt();
                string input = Console.ReadLine();
                if (input != "exit")
                {
                    ParseInput(input);
                }
                else
                {
                    break;
                }
            }
        }

        private void ShowPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Discoverer>");
            Console.ResetColor();
        }

        private void ParseInput(string input)
        {
            List<string> parts = input.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (parts.Count < 1)
            {
                Console.WriteLine();
                return;
            }

            string command = parts[0];

            switch (command)
            {
                case "help":
                    Console.WriteLine("Discoverer V1.0.0 by plymouth");
                    break;
                case "user":
                    if (parts.Count < 3)
                    {
                        Console.WriteLine("Invalid arguments");
                        break;
                    }
                    string username = parts[1];
                    string item = parts[2];
                    try
                    {
                        GetUserInfo(username, item);
                    }
                    catch (System.Net.WebException e)
                    {
                        Console.WriteLine("Not found");
                        Debug.WriteLine(e.InnerException);
                        Debug.WriteLine(e.Message);
                        Debug.WriteLine(e.StackTrace);
                    }
                    break;
                case "hipr":
                    ListHighPriorityUsers();
                    break;
                case "who":
                    ListArchivedUsers();
                    break;
                case "search":
                    if (parts.Count <2 )
                    {
                        Console.WriteLine("Invalid arguments");
                    }
                    string query = string.Join(" ", parts.Skip(1));
                    Search(query);
                    break;
                default:
                    Console.WriteLine("Unrecognized command: {0}", command);
                    break;
            }

            Console.WriteLine();
        }

        private void GetUserInfo(string username, string item)
        {
            User user = twitterEngine.GetUser(username);
            // TODO: Cache recent users

            switch (item)
            {
                case "tweets":
                    if (user.IsProtected)
                    {
                        Console.WriteLine("User is protected");
                        break;
                    }
                    Console.WriteLine("# Latest tweets");
                    foreach (Tweet tweet in user.TweetList)
                    {
                        PrintTweet(tweet);
                        Console.WriteLine();
                    }
                    break;
                case "following":
                    if (user.IsProtected)
                    {
                        Console.WriteLine("User is protected");
                        break;
                    }
                    Console.WriteLine("# Following {0} users", user.FollowingCount);
                    foreach (User following in user.FollowingList)
                    {
                        PrintName(following);
                        Console.WriteLine();
                    }
                    break;
                case "followers":
                    if (user.IsProtected)
                    {
                        Console.WriteLine("User is protected");
                        break;
                    }
                    Console.WriteLine("Followed by {0} users", user.FollowerCount);
                    foreach (User follower in user.FollowerList)
                    {
                        PrintName(follower);
                        Console.WriteLine();
                    }
                    break;
                case "favorites":
                    if (user.IsProtected)
                    {
                        Console.WriteLine("User is protected");
                        break;
                    }
                    Console.WriteLine("# {0} favorites", user.FavoriteCount);
                    foreach (Tweet favorite in user.FavoriteList)
                    {
                        PrintTweet(favorite);
                        Console.WriteLine();
                    }
                    break;
                case "archive":
                    Console.WriteLine("Archiving user info...");
                    ArchiveUser(user);
                    Console.WriteLine("Done");
                    break;
                case "info":
                    PrintUserStats(user);
                    break;
                case "mkhipr":
                    Console.WriteLine("Adding user to high priority list");
                    AddToHighPriorityList(user.UserName);
                    Console.WriteLine("Done");
                    break;
                case "viewchrome":
                    Console.WriteLine("Opening profile in Chrome...");
                    string profileUrl = string.Format("http://twitter.com/{0}", user.UserName);
                    WebUtil.OpenInChrome(profileUrl, true);
                    Console.WriteLine("Done");
                    break;
                case "viewavi":
                    Console.WriteLine("Opening avatar in Chrome...");
                    WebUtil.OpenInChrome(user.AvatarUrl, true);
                    Console.WriteLine("Done");
                    break;
                default:
                    Console.WriteLine("Invalid argument");
                    break;
            }
        }

        private void PrintTweet(Tweet tweet)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("@{0}", tweet.AuthorUsername);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(tweet.AuthorName);
            Console.ResetColor();
            Console.WriteLine(tweet.Content);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[{0}]", tweet.Id);
            Console.ResetColor();
        }

        private void PrintName(User user)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("@{0}", user.UserName);
            Console.ResetColor();
            Console.WriteLine(user.FullName);
        }

        private void PrintUserStats(User user)
        {
            PrintName(user);
            Console.WriteLine(user.AvatarUrl);
            Console.WriteLine("Location: {0}", user.Location);
            Console.WriteLine("Bio: {0}", user.Bio);
            Console.WriteLine("{0} Following", user.FollowingCount);
            Console.WriteLine("{0} Followers", user.FollowerCount);
            Console.WriteLine("{0} Favorites", user.FavoriteCount);
            Console.WriteLine("Protected account: {0}", user.IsProtected);
        }

        private void ArchiveUser(User user)
        {
            string path = Path.Combine(paths["archives"], user.UserName);
            IOUtil.CreateDirectory(path);
            Console.WriteLine("Writing to {0}", path);

            string json = JsonConvert.SerializeObject(user, Formatting.Indented);
            string fileName = String.Format("\\{0}_{1}.json", user.UserName, GetTimestamp());
            IOUtil.WriteToFile(path + fileName, json);
        }

        private void AddToHighPriorityList(string username)
        {
            string path = paths["home"];
            IOUtil.CreateDirectory(path);
            Console.WriteLine("Writing to {0}", path);

            IOUtil.AppendToFile(path + "\\high_priority.txt", username);
        }

        private void ListHighPriorityUsers()
        {
            string path = paths["home"] + "\\high_priority.txt";
            string[] lines = IOUtil.GetAllLines(path);

            PrintFormattedLines(lines);
            Console.WriteLine("\n\nHigh priority users: {0}", lines.Length);     
        }

        private void ListArchivedUsers()
        {
            string path = paths["archives"];
            string[] folderNames = IOUtil.GetFolderNames(path);

            PrintFormattedLines(folderNames);
            Console.WriteLine("\n\nUsers archived: {0}", folderNames.Length);
        }

        private void PrintFormattedLines(string[] lines)
        {
            int count = 0;
            foreach (string line in lines)
            {
                Console.Write(line.PadRight(16));

                if (++count % 4 == 0) // 4 columns per row
                {
                    count = 0;
                    Console.WriteLine();
                }
            }
        }

        private void Search(string query)
        {
            List<User> users = twitterEngine.SearchForUsers(query);

            foreach (User user in users)
            {
                PrintName(user);
                Console.WriteLine();
            }
        }

        private string GetTimestamp()
        {
            string timestamp = DateTime.Now.ToString("MMddyyyyhhmmss");
            return timestamp;
        }
    }
}
