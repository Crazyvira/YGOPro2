using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShaCache
{
    public class Sha
    {
        public Sha()
        { }
        public string path { get; set; }
        public DateTime unixTime { get; set; }
        public string SHA { get; set; }
        public override string ToString()
        {
            return string.Format("{0}->{1},{2}", path, DateTimeToUnixTimestamp(unixTime).ToString(), SHA);
        }
    }

    private List<Sha> shas { get; set; }

    public ShaCache()
    {
        if (!Directory.Exists("Update/"))
            Directory.CreateDirectory("Update/");
        if (!File.Exists("Update/SHACACHE.conf"))
            File.Create("Update/SHACACHE.conf").Close();
        GetCache();
    }

    public void GetCache()
    {
        shas = new List<Sha>();
        string[] localShas = File.ReadAllLines("Update/SHACACHE.conf");
        foreach (string str in localShas)
        {
            if (!str.Contains("->") && !str.Contains(","))
                continue;
            Sha add = new Sha();
            string[] firstPart = str.Split("->");
            add.path = firstPart[0];
            string[] secondPart = firstPart[1].Split(',');
            add.unixTime = UnixTimeStampToDateTime(Convert.ToDouble(secondPart[0]));
            add.SHA = secondPart[1];
            shas.Add(add);
        }
    }

    public void UpdateInsertCache(string filePath, string sha)
    {
        Sha newSHA = new Sha();
        newSHA.path = filePath;
        newSHA.unixTime = DateTime.Now;
        newSHA.SHA = sha;
        string write = "";
        bool found = false;
        for (int i = 0; i < shas.Count; i++)
        {
            if (shas[i].path == newSHA.path)
            { shas[i] = newSHA; found = true; }
            write += shas[i].ToString() + "\n";
        }
        if (!found)
        {
            shas.Add(newSHA);
            write += newSHA.ToString() + "\n";
        }
        File.WriteAllText("Update/SHACACHE.conf", write);
    }

    public bool MatchesCache(string filePath, string SHA)
    {
        FileInfo file = new FileInfo(filePath);
        Sha found = shas.FirstOrDefault(predicate => file.FullName.Replace("\\", "/").Contains(predicate.path));
        if (found == null || !file.Exists || found.SHA != SHA || file.LastWriteTime.Equals(found.unixTime))
            return false;
        return true;
    }

    public static string GetHashString(string filePath)
    {
        FileInfo file = new FileInfo(filePath);
        if (!file.Exists)
            return "";
        byte[] bytes1 = System.Text.Encoding.ASCII.GetBytes("blob " + file.Length.ToString() + '\0'.ToString());
        byte[] bytes2 = File.ReadAllBytes(file.FullName);
        List<byte> temp = new List<byte>();
        temp.AddRange(bytes1);
        temp.AddRange(bytes2);
        byte[] bytes = temp.ToArray();
        System.Security.Cryptography.HashAlgorithm algorithm = System.Security.Cryptography.SHA1.Create();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in algorithm.ComputeHash(bytes))
            sb.Append(b.ToString("X2"));

        return sb.ToString().ToLower();
    }

    public static string ToContaingFolder(FileInfo filePath)
    {
        string path = filePath.FullName;
        string loc = Application.dataPath.Replace("/Assets", "");
        path = path.Replace("\\", "/");
        path = path.Replace(loc + "/", "");
        path = path.Replace(temp.Name, "");
        return path;
    }

    public static string ToContaingFolder(string filePath)
    {
        FileInfo temp = new FileInfo(filePath);
        string path = temp.FullName;
        string loc = Application.dataPath.Replace("/Assets", "");
        path = path.Replace("\\", "/");
        path = path.Replace(loc + "/", "");
        path = path.Replace(temp.Name, "");
        return path;
    }

    public static double DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
               new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

}

