using System.IO;
using System.Collections.Generic;

namespace FileFormats.Hashing
{
    internal static class FileHash
    {
        private static Dictionary<ulong, string> dict = null;

        private static string file;

        public static void Load(string path = "hashlist.txt")
        {
            if (!File.Exists(path))
            {
                return;
            }

            file = path;

            dict = new Dictionary<ulong, string>();

            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string name = sr.ReadLine();
                    ulong hash = FNV64.Hash(name);

                    if (!FileHash.dict.ContainsKey(hash))
                    {
                        FileHash.dict.Add(hash, name);
                    }
                }
            }
        }

        public static void Save(string path = "hashlist.txt")
        {
            if (dict == null) return;

            using (var output = new StreamWriter(path, false))
            {
                foreach (var k in dict)
                {
                    output.WriteLine(k.Value);
                }
            }
        }

        public static void Add(string name)
        {
            if (dict == null) return;

            if (ResourceExplorer.Helpers.IsFileLocked(new FileInfo(file)) == false)
            {
                ulong hash = FNV64.Hash(name);

                if (!dict.ContainsKey(hash))
                {
                    using (var sw = new StreamWriter(file, true))
                    {
                        sw.WriteLine(name);
                    }
                }
            }
        }

        public static string Name(ulong hash)
        {
            if (dict != null)
            {
                if (dict.ContainsKey(hash))
                {
                    return dict[hash];
                }
            }

            return string.Format("0x{0:x16}", hash);
        }
    }
}
