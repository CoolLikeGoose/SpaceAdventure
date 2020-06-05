using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// A class that implements data management
/// 3 methods for [coins, score, skins] -
/// Load Save Delete
/// </summary>
public static class DataController
{
    /// <summary>
    /// Load data
    /// </summary>
    /// <param name="file">score/coins</param>
    /// <returns>saved data</returns>
    public static int LoadFile(string file)
    {
        string path = Application.persistentDataPath + $"/{file}.goose";

        if (File.Exists(path))
        {
            StreamReader data = new StreamReader(path);
            int count = Convert.ToInt32(data.ReadLine());
            data.Close();

            return count;
        }
        return 0;
    }

    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="count">data to save</param>
    /// <param name="file">score/coins</param>
    public static void SaveFile(int count, string file)
    {
        string path = Application.persistentDataPath + $"/{file}.goose";

        StreamWriter data = new StreamWriter(path);
        data.WriteLine(count);
        data.Close();
    }

    /// <summary>
    /// Delete data
    /// </summary>
    /// <param name="file">score/coins</param>
    public static void DeleteFile(string file)
    {
        string path = Application.persistentDataPath + $"/{file}.goose";

        File.Delete(path);
    }

    //special method for skins
    public static List<int> LoadBoughtSkins()
    {
        string path = Application.persistentDataPath + $"/skins.goose";
        List<int> skinsData = new List<int>();

        if (File.Exists(path))
        {
            StreamReader data = new StreamReader(path);
            string line;
            while ((line = data.ReadLine()) != null)
            {
                skinsData.Add(Convert.ToInt32(line));
            }
            data.Close();

            return skinsData;
        }
        return null;
    }

    public static void SaveBoughtSkins(int skinIndex)
    {
        string path = Application.persistentDataPath + $"/skins.goose";

        StreamWriter coinsdata = new StreamWriter(path, true);
        coinsdata.WriteLine(skinIndex);
        coinsdata.Close();
    }
}
