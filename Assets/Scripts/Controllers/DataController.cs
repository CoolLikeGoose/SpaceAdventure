using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// A class that implements data management
/// 3 methods for [coins, score] -
/// Load Save Delete
/// </summary>
public static class DataController
{
    //max score
    public static int LoadScore()
    {
        string path = Application.persistentDataPath + "/score.goose";

        if (File.Exists(path))
        {
            StreamReader scoredata = new StreamReader(path);
            int score = Convert.ToInt32(scoredata.ReadLine());
            scoredata.Close();

            return score;
        }
        return 0;
    }

    public static void SaveScore(int score)
    {
        string path = Application.persistentDataPath + "/score.goose";

        StreamWriter scoredata = new StreamWriter(path);
        scoredata.WriteLine(score);
        scoredata.Close();
    }

    public static void DeleteScore()
    {
        string path = Application.persistentDataPath + "/score.goose";

        File.Delete(path);
    }

    //coins
    public static int LoadCoins()
    {
        string path = Application.persistentDataPath + "/coins.goose";

        if (File.Exists(path))
        {
            StreamReader coinsdata = new StreamReader(path);
            int coins = Convert.ToInt32(coinsdata.ReadLine());
            coinsdata.Close();

            return coins;
        }
        return 0;
    }

    public static void SaveCoins(int coins)
    {
        string path = Application.persistentDataPath + "/coins.goose";

        StreamWriter coinsdata = new StreamWriter(path);
        coinsdata.WriteLine(coins);
        coinsdata.Close();
    }

    public static void DeleteCoins()
    {
        string path = Application.persistentDataPath + "/coins.goose";

        File.Delete(path);
    }
    
}
