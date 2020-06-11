using System.Collections;
using UnityEngine;

/// <summary>
/// Manages Enemy movement/shooting/destruction
/// </summary>
public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject Laser = null;

    //if enemy has fire ability
    [SerializeField] private bool canFire = false;

    //for small-tripple enemy
    [SerializeField] private bool cantMoveByHimself = false;

    //for big enemy
    [SerializeField] private bool haveHp = false;

    //for the enemy the movement of which is z-shaped
    [SerializeField] private bool fastMove = false;

    private float speed;
    private GameObject[] loot;
    private GameObject damage;

    private Renderer r;

    private int hp = 3;
    private int movDirect;

    private void Start()
    {
        //if object is inVisible sounds will not play
        r = GetComponent<Renderer>();

        speed = GameController.Instance.EnemySpeed;

        loot = GameController.Instance.enemyLoot;

        damage = GameController.Instance.shipHitParticles;

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

    /// <summary>
    /// Coroutine that controls weapon shooting
    /// </summary>
    private IEnumerator WeaponShoot()
    {
        if (r.isVisible) { SoundController.Instance.LaserShot(1); }
        
        Instantiate(Laser, new Vector2(transform.position.x, transform.position.y - .6f), Quaternion.identity);

        yield return new WaitForSeconds(GameController.Instance.enemyShootDelay/GameController.Instance.gameSpeed);
        StartCoroutine(WeaponShoot());
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

    /// <summary>
    /// Reduce hp from big enemy and create particles
    /// </summary>
    private void DamageEnemy()
    {
        hp -= GameController.Instance.playerDamage;
        if (hp <= 0) { DestroyEnemy(); }

        Instantiate(damage, transform.position, Quaternion.identity);
    }

    public void DestroyEnemy()
    {
        GameController.Instance.score++;

        //explosion
        GameObject explosion = Instantiate(GameController.Instance.shipExplosion, transform.position, Quaternion.identity);
        if (cantMoveByHimself) { explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); }

        //loot if enemy != enemySmall
        else { if (Random.Range(0, 100) < 35) { Instantiate(loot[Random.Range(0, loot.Length)], transform.position, Quaternion.identity); } }

        //sound
        if (r.isVisible) { SoundController.Instance.EnemyExplosion(); }

        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the enemy on z-shaped path
    /// </summary>
    private void EnemyAdditionalMovement()
    {
        if ((movDirect == -1 && transform.position.x > 2.2f) || (movDirect == 1 && transform.position.x < -2.2f))
        {
            movDirect *= -1;
        }

        transform.Translate(new Vector2(speed * Time.deltaTime * movDirect, 0));
    }
}
