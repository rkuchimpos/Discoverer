
using System.IO;

namespace Discoverer
{
    public static class IOUtil
    {
        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void WriteToFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public static void AppendToFile(string path, string content)
        {
            using (StreamWriter writer = File.AppendText(path))
            {
                writer.WriteLine(content);
            }
        }

        public static string[] GetAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public static string[] GetFolderNames(string path)
        {
            string[] folders = Directory.GetDirectories(path);
            string[] folderNames = new string[folders.Length];

            for (int i = 0; i < folders.Length; i++)
            {
                string folderName = folders[i].Remove(0, folders[i].LastIndexOf("\\") + 1);
                folderNames[i] = folderName;
            }

            return folderNames;
        }
    }
}
