using System;
using System.IO;
using System.Text;
using Gibbed.Helpers;
using System.Collections.Generic;
using Gibbed.Illusion.FileFormats;

namespace Gibbed.Illusion.FileFormats.Hashes
{
    public class M3Hash
    {
        public string name;
        public uint hash;
    }

    public class M3HashList
    {
        public List<M3Hash> hashedList;
        public void Initialize()
        {
            hashedList = new List<M3Hash>();

            using (var sr = new StreamReader("hash.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();

                    Array.Resize(ref hashList, hashList.Length + 1);
                    hashList[hashList.Length - 1] = s;
                }
            }


            for (var i = 0; i < hashList.Length; i++)
            {
                var hash = hashList[i];
                byte[] stringbytes = Encoding.ASCII.GetBytes(hash);
                M3Hash hashItem = new M3Hash();
                hashItem.name = hash;
                hashItem.hash = FNV.Hash32(stringbytes, 0, stringbytes.Length);
                hashedList.Add(hashItem);
            }
        }

        public string GetStringByHash(uint hash)
        {
            for (var i = 0; i < hashedList.Count; i++)
            {
                var hashItem = hashedList[i];
                if(hashItem.hash == hash)
                {
                    return hashItem.name;
                }
            }

            return null;
        }

        public static string[] hashList =
        {
            "test"
        };
    }
}
