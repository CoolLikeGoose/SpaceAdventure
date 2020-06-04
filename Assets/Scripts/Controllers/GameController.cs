﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Main Controller, contains all main settings for customizing the game
/// </summary>
public class GameController : MonoBehaviour
{
    //enemy settings
    public float EnemySpeed = -.1f;
    public float enemySpawnDelay = .25f;
    public float enemyShootDelay = .5f;
    public float enemyLaserSpeed = -.2f;

    //player settings
    public float playerShootDelay = .25f;
    public float playerLaserSpeed = .2f;

    public GameObject asteroidExplosion;

    //loot
    public GameObject[] enemyLoot;
    public GameObject coinPref;

    //Game speedup during the game
    [SerializeField] private float speedUpFactor = 1;
    [NonSerialized] public float gameSpeed = 1;

    //
    [NonSerialized] public bool isGameActive = true;

    //score and coins
    public int score    
    {
        get { return _score; }
        set
        {
            if (isGameActive)
            {
                _score = value;
                GUIController.Instance.gameScoreSet = _score;
            }

        }
    }
    private int _score = 0;
    [NonSerialized] public int maxScore;
    [NonSerialized] public int coins = 0;

    //music DELETE
    [NonSerialized] public bool fxMuted = false;
    [NonSerialized] public bool musicMuted = false;

    //check if now scene == menu
    [NonSerialized] public bool nowSceneMenu;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        nowSceneMenu = SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0);

        Instance = this;

        maxScore = DataController.LoadScore();
        coins = DataController.LoadCoins();
    }
    
    private void Start()
    {
        //display maxScore and coins if now screen - menu
        if (nowSceneMenu)
        {
            GUIController.Instance.scoreSet = maxScore;
            GUIController.Instance.coinSet = coins;
        }

        //mute FX/music if you muted them before
        GUIController.Instance.OnFXMute(PlayerPrefs.GetInt("fxMuted"));
        GUIController.Instance.OnMusicMute(PlayerPrefs.GetInt("musicMuted"));
    }

    private void Update()
    {
        //speedup; max speedup = 5
        if (!nowSceneMenu && gameSpeed < 5) { gameSpeed += speedUpFactor; }
        Debug.Log(gameSpeed);
    }

    //needs to call coroutine 
    /// <summary>
    /// Called when the player is dead or went to the menu
    /// </summary>
    /// <param name="mode">"death" for standart game over (with death screen and and a two second pause)</param>
    public void GameOver(string mode)
    {
        DataController.SaveCoins(coins);
        StartCoroutine(GameOverCor(mode));
    }

    public IEnumerator GameOverCor(string mode)
    {
        if (mode == "death")
        {
            GUIController.Instance.gameOver = _score;

            isGameActive = false;
            Handheld.Vibrate();

            yield return new WaitForSeconds(2f);
        }
        
        GUIController.Instance.OnMenuBtn();
    }
}
