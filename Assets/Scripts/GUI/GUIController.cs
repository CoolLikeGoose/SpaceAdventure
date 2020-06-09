using System;
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
    [SerializeField] private GameObject info = null;

    [SerializeField] private Sprite unMuteIcon = null;
    [SerializeField] private Sprite muteIcon = null;

    [SerializeField] private Image ObjFXmuteIcon = null;
    [SerializeField] private Image ObjMusicMuteIcon = null;

    [SerializeField] private GameObject gameOverPopup = null;

    [SerializeField] private Text scoreLabel = null;
    [SerializeField] private Text coinsLabel = null;
    [SerializeField] private Text finalScoreLabel = null;

    [SerializeField] private Image reloadSprite;
    [SerializeField] private Image reloadSpriteBackground;
    [SerializeField] private Sprite reloadSpriteDrones = null;
    [SerializeField] private Sprite reloadSpriteShield = null;
    [SerializeField] private Sprite reloadSpriteBoost = null;


    // TODO: create another class for this
    [NonSerialized] public bool isAbilityActivated;

    //some properties for update GUI
    public int gameOver
    {
        set
        {
            main.SetActive(false);
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
        set
        {
            if (GameController.Instance.nowSceneMenu) { coinsLabel.text = $"x {value}"; }
            else { coinsLabel.text = value.ToString(); }
        }
    }

    public int reloadSpriteSet
    {
        set
        {
            if (value == 0)
            {
                reloadSprite.sprite = reloadSpriteDrones;
                reloadSpriteBackground.sprite = reloadSpriteDrones;
            }
            else if (value == 1)
            {
                reloadSprite.sprite = reloadSpriteShield;
                reloadSpriteBackground.sprite = reloadSpriteShield;
            }
            else
            {
                reloadSprite.sprite = reloadSpriteBoost;
                reloadSpriteBackground.sprite = reloadSpriteBoost;
            }
        }
    }

    private GameObject activeWindow;

 
    private void Awake()
    {
        Instance = this;
        activeWindow = main;
        //reloadSprite.sprite = reloadSpriteDrones;
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
        GameController.Instance.coins = 0;
    }

    public void OnDeleteSkinsBtn()
    {
        ShopController.Instance.OnDeleteSkins();
    }

    //ship superAbility
    public void OnSuperAbility()
    {
        if (!isAbilityActivated) { StartCoroutine(UsingSuperAbility()); }
    }

    public IEnumerator ReloadSuperAbility()
    {
        reloadSprite.fillAmount = 0;

        reloadSprite.color = new Color(255, 255, 255);

        while (reloadSprite.fillAmount < 1)
        {
            //reload ~40s
            reloadSprite.fillAmount += 0.001f / 3f;
            yield return new WaitForSeconds(0.01f);
        }
        isAbilityActivated = false;

        reloadSprite.color = new Color(255, 255, 0);
    }

    private IEnumerator UsingSuperAbility()
    {
        //PlayerController.Instance.SuperShieldActivate();
        SuperAbilityController.Instance.ActivateAbility();

        isAbilityActivated = true;

        while (reloadSprite.fillAmount > 0)
        {
            //using ~10s
            reloadSprite.fillAmount -= 0.001f;
            yield return new WaitForSeconds(0.01f);
        }

        //PlayerController.Instance.SuperShieldDown();
        SuperAbilityController.Instance.DeactivateAbility();

        StartCoroutine(ReloadSuperAbility());
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
            case "info":
                activeWindow = info;
                turnCoinsLabel(false);
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
