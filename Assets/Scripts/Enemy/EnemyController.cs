using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manages Enemy movement/shooting/destruction
/// </summary>
public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject Laser = null;

    private float speed;
    private GameObject[] loot;

    private Renderer r;

    private void Start()
    {
        //if object is inVisible sounds will not play
        r = GetComponent<Renderer>();

        speed = GameController.Instance.EnemySpeed;

        loot = GameController.Instance.enemyLoot;

        StartCoroutine(WeaponShoot());
    }

    private void Update()
    {
        transform.Translate(new Vector2(0, speed * Time.deltaTime * 1.2f));
    }

    private IEnumerator WeaponShoot()
    {
        if (r.isVisible) { SoundController.Instance.LaserShot(1); }
        
        Instantiate(Laser, new Vector2(transform.position.x, transform.position.y - .6f), Quaternion.identity);

        if (GameController.Instance.isGameActive)
        {
            yield return new WaitForSeconds(GameController.Instance.enemyShootDelay);
            StartCoroutine(WeaponShoot());
        }

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "laser")
        {
            GameController.Instance.score++;

            Destroy(collision.gameObject);

            //explosion
            Instantiate(GameController.Instance.asteroidExplosion, transform.position, Quaternion.identity);

            //loot
            if (Random.Range(0,100) < 25) { Instantiate(loot[Random.Range(0, loot.Length)], transform.position, Quaternion.identity); }
            
            //sound
            if (r.isVisible) { SoundController.Instance.EnemyExplosion(); }

            Destroy(gameObject);    
        }
    }
}
