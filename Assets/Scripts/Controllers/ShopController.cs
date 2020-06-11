using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }

    [Tooltip("The index of the buttons in the array must match their number")]
    [SerializeField] private List<GameObject> shopButtons = null;
    [Tooltip("The index of the ships in the array must match their number")]
    public List<GameObject> shipSkins;

    private GameObject nowActiveBtn = null;

    private int[] oneDamageShipsIndex = { 0, 3, 6};
    private int[] twoDamageShipsIndex = { 1, 4, 7};

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //glow up active ship
        GlowSelectedSkin(PlayerPrefs.GetInt("activeShipSkin", 0));

        //remove price labels from already purchased skins 
        List<int> loadedSkins = DataController.LoadBoughtSkins();
        if (loadedSkins == null)
        {
            SetDefaultSkin();
            return;
        }
        foreach (int skinIndex in loadedSkins)
        {
            //Reduce cost labels if ship is already bought
            shopButtons[skinIndex].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void BuySkin(int skinIndex)
    {
        //check if already bought and select them
        List<int> loadedSkins = DataController.LoadBoughtSkins();
        if (loadedSkins != null && loadedSkins.Contains(skinIndex)) { SaveSkinChange(skinIndex); return; }

        //buy process
        int cost = Convert.ToInt32(shopButtons[skinIndex].GetComponentInChildren<Text>().text);
        if (cost != 0 && GameController.Instance.coins >= cost)
        {
            SaveSkinChange(skinIndex);

            GameController.Instance.coins -= cost;
            GUIController.Instance.coinSet = GameController.Instance.coins;
            DataController.SaveFile(GameController.Instance.coins, "coins");

            //Reduce cost labels
            nowActiveBtn.transform.GetChild(1).gameObject.SetActive(false);

            DataController.SaveBoughtSkins(skinIndex);

            return;
        }
    }

    public void GlowSelectedSkin(int skinIndex)
    {
        if (nowActiveBtn != null)
        {
            nowActiveBtn.GetComponent<Image>().color = new Color(255, 255, 255);
        }
        nowActiveBtn = shopButtons[skinIndex];
        nowActiveBtn.GetComponent<Image>().color = new Color(255, 205, 0);
    }

    /// <summary>
    /// Set damage and save skin Index in PlayerPrefs
    /// </summary>
    public void SaveSkinChange(int skinIndex)
    {
        GlowSelectedSkin(skinIndex);

        //set damage
        int damage;
        if (oneDamageShipsIndex.Contains(skinIndex)) { damage = 1; }
        else if (twoDamageShipsIndex.Contains(skinIndex)) { damage = 2; }
        else { damage = 3; }
        PlayerPrefs.SetInt("playerDamage", damage);

        PlayerPrefs.SetInt("activeShipSkin", skinIndex);
        PlayerPrefs.Save();
    }

    //Invoke from GUIController
    public void OnDeleteSkins()
    {
        //searh for all already purchased and Activate them price labels 
        List<int> loadedSkins = DataController.LoadBoughtSkins();
        if (loadedSkins == null) { return; }
        foreach (int skinIndex in loadedSkins)
        {
            shopButtons[skinIndex].transform.GetChild(1).gameObject.SetActive(true);
        }

        DataController.DeleteFile("skins");

        SetDefaultSkin();
    }

    //Set 1 skin as Default
    private void SetDefaultSkin()
    {
        shopButtons[0].transform.GetChild(1).gameObject.SetActive(false);
        GlowSelectedSkin(0);
        DataController.SaveBoughtSkins(0);
        SaveSkinChange(0);
    }
}