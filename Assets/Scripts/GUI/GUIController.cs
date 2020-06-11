using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages all changes in the interface
/// </summary>
public class GUIController : MonoBehaviour
{
    public static GUIController Instance { get; private set; }

    //windows
    [SerializeField] private GameObject main = null;
    [SerializeField] private GameObject settings = null;
    [SerializeField] private GameObject shop = null;
    [SerializeField] private GameObject info = null;
    [SerializeField] private GameObject gameOverPopup = null;

    //Icons for sound settings
    [SerializeField] private Sprite unMuteIcon = null;
    [SerializeField] private Sprite muteIcon = null;

    //Btns Images to change 
    [SerializeField] private Image ObjFXmuteIcon = null;
    [SerializeField] private Image ObjMusicMuteIcon = null;

    //Stats labels
    [SerializeField] private Text scoreLabel = null;
    [SerializeField] private Text coinsLabel = null;
    [SerializeField] private Text finalScoreLabel = null;

    //Ability btns Images that indicate ability state
    [SerializeField] private Image reloadSprite = null;
    [SerializeField] private Image reloadSpriteBackground = null;

    //Ability sprites
    [SerializeField] private Sprite reloadSpriteDrones = null;
    [SerializeField] private Sprite reloadSpriteShield = null;
    [SerializeField] private Sprite reloadSpriteBoost = null;

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
            if (value < 1000) { scoreLabel.text = $"score\n{value:000}"; }
            else { scoreLabel.text = $"score\n{value}"; }
        }
    }
    public int coinSet
    {
        set
        {
            if (GameController.Instance.nowSceneMenu) { coinsLabel.text = $"x {value}"; }
            else if (value < 1000) { coinsLabel.text = value.ToString(); }
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

    //Ship super ability animations
    public void OnSuperAbility()
    {
        if (!SuperAbilityController.Instance.isAbilityActivated) { StartCoroutine(UsingSuperAbility()); }
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
        SuperAbilityController.Instance.isAbilityActivated = false;

        reloadSprite.color = new Color(255, 255, 0);
    }

    private IEnumerator UsingSuperAbility()
    {
        SuperAbilityController.Instance.ActivateAbility();

        SuperAbilityController.Instance.isAbilityActivated = true;

        while (reloadSprite.fillAmount > 0)
        {
            //using ~10s
            reloadSprite.fillAmount -= 0.001f;
            yield return new WaitForSeconds(0.01f);
        }

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

    //Hide or show Coins label
    //if condition necessary that when the method is called not from the main menu, an error does not occur
    private void turnCoinsLabel(bool state)
    {
        if (GameController.Instance.nowSceneMenu)
        {
            coinsLabel.transform.parent.gameObject.SetActive(state);
        }
    }

    /// <summary>
    /// Mute music and display that on GUI
    /// </summary>
    /// <param name="state">0 equal false(OFF); 1 equal true(ON); 2 to get current state from audio source</param>
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

        ObjMusicMuteIcon.sprite = nowSprite;

        PlayerPrefs.SetInt("musicMuted", state);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Mute sound FX and display that on GUI
    /// </summary>
    /// <param name="state">0 equal false(OFF; 1 equal true(ON); 2 to get current state from audio source</param>
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
