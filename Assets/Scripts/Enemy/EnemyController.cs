using System.Collections;
using UnityEngine;

/// <summary>
/// Manages Enemy movement/shooting/destruction
/// </summary>
public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject Laser = null;
    [SerializeField] private bool canFire;
    [SerializeField] private bool cantMoveByHimself;
    [SerializeField] private bool haveHp;
    [SerializeField] private bool fastMove;

    private float speed;
    private GameObject[] loot;

    private Renderer r;

    private int hp = 3;
    private int movDirect;

    private void Start()
    {
        //if object is inVisible sounds will not play
        r = GetComponent<Renderer>();

        speed = GameController.Instance.EnemySpeed;

        loot = GameController.Instance.enemyLoot;

        if (canFire) { StartCoroutine(WeaponShoot()); }
        if (fastMove)
        {
            movDirect = Random.Range(0, 1);
            if (movDirect == 0) { movDirect = -1; }
        }
    }

    private void Update()
    {
        if (!cantMoveByHimself) { transform.Translate(new Vector2(0, speed * Time.deltaTime * 1.2f * GameController.Instance.gameSpeed)); }
        if (fastMove) { EnemyAdditionalMovement(); }
    }

    private IEnumerator WeaponShoot()
    {
        if (r.isVisible) { SoundController.Instance.LaserShot(1); }
        
        Instantiate(Laser, new Vector2(transform.position.x, transform.position.y - .6f), Quaternion.identity);

        if (GameController.Instance.isGameActive)
        {
            yield return new WaitForSeconds(GameController.Instance.enemyShootDelay/GameController.Instance.gameSpeed);
            StartCoroutine(WeaponShoot());
        }

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "laser")
        {
            Destroy(collision.gameObject);

            if (haveHp) { DamageEnemy(); }
            else { DestroyEnemy(); }
        }
    }

    private void DamageEnemy()
    {
        hp -= GameController.Instance.playerDamage;
        if (hp <= 0) { DestroyEnemy(); }
    }

    public void DestroyEnemy()
    {
        GameController.Instance.score++;

        //explosion
        GameObject explosion = Instantiate(GameController.Instance.asteroidExplosion, transform.position, Quaternion.identity);
        if (cantMoveByHimself) { explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); }

        //loot if enemy != enemySmall
        else { if (Random.Range(0, 100) < 25) { Instantiate(loot[Random.Range(0, loot.Length)], transform.position, Quaternion.identity); } }

        //sound
        if (r.isVisible) { SoundController.Instance.EnemyExplosion(); }

        Destroy(gameObject);
    }

    private void EnemyAdditionalMovement()
    {
        if ((movDirect == -1 && transform.position.x > 2.2f) || (movDirect == 1 && transform.position.x < -2.2f))
        {
            movDirect *= -1;
            Debug.Log("Change");
        }

        transform.Translate(new Vector2(speed * Time.deltaTime * movDirect, 0));
    }
}
