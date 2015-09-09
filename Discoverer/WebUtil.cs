using System.Diagnostics;
using System.Net;

namespace Discoverer
{
    internal static class WebUtil
    {
        public static string GetSource(string url)
        {
            return (new WebClient()).DownloadString(url);
        }

        public static void OpenInChrome(string url, bool incognito)
        {
            Process chrome = new Process();
            chrome.StartInfo.FileName = "chrome.exe";
            chrome.StartInfo.Arguments = incognito ? "--incognito " + url : url;
            chrome.Start();
        }
    }
}