using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages player movement/shoot/collect something
/// </summary>
public class PlayerController : MonoBehaviour
{
    //[SerializeField] private int skinIndex;

    public static PlayerController Instance { get; private set; }

    private float smoothFactor = 7f;

    private GameObject[] weapons;
    [NonSerialized] public GameObject[] shieldsPrefs;

    [NonSerialized] public int curWeaponIndex = 0;
    private int bulletsLeft = 0;

    [NonSerialized] public int curShieldIndex = -1;
    [NonSerialized] public GameObject curShield = null;

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
        //if (BossController.Instance != null && !BossController.Instance.isFighting) { yield return new WaitUntil(() => BossController.Instance.isFighting); }

        if (curWeaponIndex != 6 && curWeaponIndex != 7)
        {
            if (bulletsLeft <= 0) { curWeaponIndex = 0; }
            else { bulletsLeft--; }
        }

        SoundController.Instance.LaserShot(0);

        Instantiate(weapons[curWeaponIndex], new Vector2(transform.position.x, transform.position.y + .2f), Quaternion.identity);

        yield return new WaitForSeconds(GameController.Instance.playerShootDelay);
        StartCoroutine(WeaponShoot());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("laser") || collision.CompareTag("GameController")) { return; }
        else if (collision.CompareTag("boss"))
        {
            GameController.Instance.GameOver("death");
            Instantiate(GameController.Instance.shipExplosion, transform.position, Quaternion.identity);
            SoundController.Instance.PlayerExplosion();
            Destroy(gameObject);

            return;
        }
        else if (collision.CompareTag("shield") && curShieldIndex != 3)
        {
            Destroy(collision.gameObject);
            OnShieldUp();
            return;
        }
        else if (collision.CompareTag("capsule") && (curWeaponIndex == 6 || curWeaponIndex == 7)) { return; }
        else if (collision.CompareTag("capsule"))
        {
            CapsuleMov data = collision.gameObject.GetComponent<CapsuleMov>();
            curWeaponIndex = data.carryWeaponIndex;
            bulletsLeft = data.amountBullets;

            Destroy(collision.gameObject);

            SoundController.Instance.GearUp();

            return;
        }
        else if (collision.CompareTag("coin"))
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
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (asteroid != null) { asteroid.DestroyAsteroid(); }
            else if (enemy != null) { enemy.DestroyEnemy(); }
            else { Destroy(collision.gameObject); }
            
            OnShieldDown();

            Handheld.Vibrate();
            return;
        }

        GameController.Instance.GameOver("death");

        Destroy(collision.gameObject);

        //explosion
        Instantiate(GameController.Instance.shipExplosion, transform.position, Quaternion.identity);

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

        SoundController.Instance.Shield(true);
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

        SoundController.Instance.Shield(false);
    }
}   
