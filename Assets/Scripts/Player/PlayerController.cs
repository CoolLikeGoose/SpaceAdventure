using System;
using System.Collections;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages player movement/shoot/collect something
/// </summary>
public class PlayerController : MonoBehaviour
{
    //[SerializeField] private int skinIndex;

    public static PlayerController Instance { get; private set; }

    [SerializeField] private float smoothFactor = 7f;

    private GameObject[] weapons;
    private GameObject[] shieldsPrefs;

    private int curWeaponIndex = 0;
    private int bulletsLeft = 0;

    private int curShieldIndex = -1;
    private GameObject curShield = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        weapons = GameController.Instance.weapons;
        shieldsPrefs = GameController.Instance.shieldsPrefs;
        StartCoroutine(WeaponShoot());
    }

    private void Update()
    {
        // TODO: fix this (ship stuck in ability btn)
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) && (!EventSystem.current.IsPointerOverGameObject(0) && !EventSystem.current.IsPointerOverGameObject() || 
            (pos.y < -3 && GUIController.Instance.isAbilityActivated)))
        {
            pos = new Vector2(pos.x, pos.y + 1f);
            transform.position = Vector2.Lerp(transform.position, pos, smoothFactor * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Escape)) { GameController.Instance.GameOver("instantly"); }
    }

    private IEnumerator WeaponShoot()
    {
        if (bulletsLeft <= 0) { curWeaponIndex = 0; }
        else { bulletsLeft--; }

        SoundController.Instance.LaserShot(0);

        Instantiate(weapons[curWeaponIndex], new Vector2(transform.position.x, transform.position.y + .2f), Quaternion.identity);

        if (GameController.Instance.isGameActive)
        {
            yield return new WaitForSeconds(GameController.Instance.playerShootDelay);
            StartCoroutine(WeaponShoot());
        }

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "laser" || collision.tag == "GameController") { return; }
        else if (collision.tag == "shield" && curShieldIndex != 3)
        {
            Destroy(collision.gameObject);
            OnShieldUp();
            return;
        }
        else if (collision.tag == "capsule")
        {
            CapsuleMov data = collision.gameObject.GetComponent<CapsuleMov>();
            curWeaponIndex = data.carryWeaponIndex;
            bulletsLeft = data.amountBullets;

            Destroy(collision.gameObject);

            SoundController.Instance.GearUp();

            return;
        }
        else if (collision.tag == "coin")
        {
            Destroy(collision.gameObject);
            GameController.Instance.coins++;

            SoundController.Instance.CoinCollected();

            return;
        }

        if (curShieldIndex == 3) { return; }
        if (curShieldIndex != -1)
        {
            AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();
            if (asteroid != null)
            {
                asteroid.DestroyAsteroid();
            }
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.DestroyEnemy();
            }
            OnShieldDown();

            Handheld.Vibrate();
            return;
        }

        GameController.Instance.GameOver("death");

        Destroy(collision.gameObject);

        //explosion
        Instantiate(GameController.Instance.asteroidExplosion, transform.position, Quaternion.identity);

        //sound
        SoundController.Instance.PlayerExplosion();

        Destroy(gameObject);
    }

    public void OnShieldUp()
    {
        if (curShield != null) { Destroy(curShield.gameObject); }
        
        curShieldIndex = Mathf.Clamp(curShieldIndex + 1, -1, 2);

        GameObject shield = Instantiate(shieldsPrefs[curShieldIndex], transform.position, Quaternion.identity);
        shield.transform.SetParent(gameObject.transform);

        curShield = shield;
    }

    public void OnShieldDown()
    {
        Destroy(curShield.gameObject);

        curShieldIndex = Mathf.Clamp(curShieldIndex - 1, -1, 2);

        if (curShieldIndex != -1)
        {
            GameObject shield = Instantiate(shieldsPrefs[curShieldIndex], transform.position, Quaternion.identity);
            shield.transform.SetParent(gameObject.transform);

            curShield = shield;
        }
    }

    public void SuperShieldActivate()
    {
        if (curShield != null) { Destroy(curShield.gameObject); }

        curShieldIndex = 3;
        GameObject shield = Instantiate(shieldsPrefs[curShieldIndex], transform.position, Quaternion.identity);
        shield.transform.SetParent(gameObject.transform);

        curShield = shield;
    }

    public void SuperShieldDown()
    {
        Destroy(curShield.gameObject);

        curShieldIndex = -1;
    }
}   
