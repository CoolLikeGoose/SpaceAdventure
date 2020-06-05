using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

/// <summary>
/// Manages all changes in the interface
/// </summary>
public class GUIController : MonoBehaviour
{
    //

    public static GUIController Instance { get; private set; }

    [SerializeField] private GameObject main = null;
    [SerializeField] private GameObject settings = null;
    [SerializeField] private GameObject shop = null;

    [SerializeField] private Sprite unMuteIcon = null;
    [SerializeField] private Sprite muteIcon = null;

    [SerializeField] private Image ObjFXmuteIcon = null;
    [SerializeField] private Image ObjMusicMuteIcon = null;

    [SerializeField] private GameObject gameOverPopup = null;

    [SerializeField] private Text scoreLabel = null;
    [SerializeField] private Text coinsLabel = null;
    [SerializeField] private Text finalScoreLabel = null;

    //some properties for update GUI
    public int gameOver
    {
        set
        {
            gameOverPopup.SetActive(true);
            finalScoreLabel.text = $"SCORE: {value}";
        }
    }
    public int scoreSet
    {
        set { scoreLabel.text = $"MAX DESTROYED: {value}"; }
    }
    public int gameScoreSet
    {
        set
        {
            if (value < 100) { scoreLabel.text = $"score\n{value.ToString("000")}"; }
            else { scoreLabel.text = $"score\n{value}"; }
        }
    }
    public int coinSet
    {
        set { coinsLabel.text = $"x {value}"; }
    }

    private GameObject activeWindow;

 
    private void Awake()
    {
        Instance = this;
        activeWindow = main;
    }

    //Buttons methods
    public void OnPlayBtn()
    {
        SceneManager.LoadScene(1);
    }
    public void OnMenuBtn()
    {
        if (GameController.Instance.maxScore < GameController.Instance.score) { DataController.SaveFile(GameController.Instance.score, "score"); }
        Time.timeScale = 1;

        DataController.SaveFile(GameController.Instance.coins, "coins");

        SceneManager.LoadScene(0);
    }
    public void OnExitBtn()
    {
        Application.Quit();
    }
       
    public void OnDeleteScoreBtn()
    {
        DataController.DeleteFile("score");
        scoreLabel.text = "MAX DESTROYED: 0";
    }

    public void OnDeleteCoinsBtn()
    {
        DataController.DeleteFile("coins");
        coinsLabel.text = "x0";
    }

    public void OnDeleteSkinsBtn()
    {
        ShopController.Instance.OnDeleteSkins();
    }

    public void OnChangeInterfaceBtn(string window)
    {
        if (!GameController.Instance.nowSceneMenu)
        {
            if (window == "settings") { Time.timeScale = 0; }
            else { Time.timeScale = 1; }
        }

        activeWindow.SetActive(false);
        switch (window)
        {
            case "main":
                activeWindow = main;
                turnCoinsLabel(true);
                break;
            case "settings":
                activeWindow = settings;
                turnCoinsLabel(false);
                break;
            case "shop":
                activeWindow = shop;
                turnCoinsLabel(true);
                break;
        }
        activeWindow.SetActive(true);
    }
    
    private void turnCoinsLabel(bool state)
    {
        if (GameController.Instance.nowSceneMenu)
        {
            coinsLabel.transform.parent.gameObject.SetActive(state);
        }
    }
    /// <summary>
    /// Mute music
    /// </summary>
    /// <param name="state">0 equal false; 1 equal true; 2 to get current state from audio source</param>
    public void OnMusicMute(int state)
    {

        Sprite nowSprite = muteIcon;

        if (state == 2)
        {
            if (SoundController.Instance.musicMute) { state = 1; }
            else { state = 0; }
        }
        if (state == 1)
        {
            nowSprite = unMuteIcon;
            SoundController.Instance.musicMute = false;
        }
        else
        {
            SoundController.Instance.musicMute = true;
        }

        //if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("menu")) { ObjMusicMuteIcon.sprite = nowSprite; };
        ObjMusicMuteIcon.sprite = nowSprite;

        PlayerPrefs.SetInt("musicMuted", state);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Mute sound FX
    /// </summary>
    /// <param name="state">0 equal false; 1 equal true; 2 to get current state from audio source</param>
    public void OnFXMute(int state)
    {

        Sprite nowSprite = muteIcon;

        if (state == 2)
        {
            if (SoundController.Instance.soundFXMute) { state = 1; }
            else { state = 0; }
        }

        if (state == 1)
        {
            nowSprite = unMuteIcon;
            SoundController.Instance.soundFXMute = false;
        }
        else
        {
            SoundController.Instance.soundFXMute = true;
        }

        //if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("menu")) { ObjFXmuteIcon.sprite = nowSprite; };
        ObjFXmuteIcon.sprite = nowSprite;

        PlayerPrefs.SetInt("fxMuted", state);
        PlayerPrefs.Save();
    }

    //skin choice 
    public void OnSkinChangedBtn(int skinIndex)
    {
        ShopController.Instance.BuySkin(skinIndex);
    }
}
