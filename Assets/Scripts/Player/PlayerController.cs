using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages player movement/shoot/collect something
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private int skinIndex;

    //[SerializeField] private GameObject[] weapons = null;
    [SerializeField] private float smoothFactor = 7f;

    private GameObject[] weapons;

    private int curWeaponIndex = 0;
    private int bulletsLeft = 0;

    private void Start()
    {
        weapons = GameController.Instance.weapons;
        StartCoroutine(WeaponShoot());
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

        GameController.Instance.GameOver("death");

        Destroy(collision.gameObject);

        //explosion
        Instantiate(GameController.Instance.asteroidExplosion, transform.position, Quaternion.identity);

        //sound
        SoundController.Instance.PlayerExplosion();

        Destroy(gameObject);
    }
}   
