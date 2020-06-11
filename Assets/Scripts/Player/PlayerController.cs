using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages player movement/shoot/collect something
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

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
        // ship movement
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) && (!EventSystem.current.IsPointerOverGameObject(0) && !EventSystem.current.IsPointerOverGameObject() || 
            (pos.y < -3 && SuperAbilityController.Instance.isAbilityActivated))) // ship shouldnt get stuck in the button, if Ability is not ready
        {
            pos = new Vector2(pos.x, pos.y + 1f);
            transform.position = Vector2.Lerp(transform.position, pos, 7f * Time.deltaTime); // the ship shouldn't teleport to finger
        }
        if (Input.GetKey(KeyCode.Escape)) { GameController.Instance.GameOver("instantly"); }
    }

    /// <summary>
    /// Coroutine that controls weapon shooting
    /// </summary>
    private IEnumerator WeaponShoot()
    {
        //if ability change the players weapon, the player mustnt change the weapon himself
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
