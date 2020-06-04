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
            StreamReader coinsdata = new StreamReader(path);
            int count = Convert.ToInt32(coinsdata.ReadLine());
            coinsdata.Close();

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
        string path = Application.persistentDataPath + $"/{count}.goose";

        StreamWriter coinsdata = new StreamWriter(path);
        coinsdata.WriteLine(count);
        coinsdata.Close();
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
}
