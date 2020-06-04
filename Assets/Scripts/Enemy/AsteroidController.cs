using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages Asteroid movement/rotation/destruction
/// </summary>
public class AsteroidController : MonoBehaviour
{
    private float speed;
    private Transform child;

    private float randomRotation;

    private GameObject explosion;

    private Renderer r;

    private void Start()
    {
        //if object is inVisible sounds will not play
        r = GetComponentInChildren<Renderer>();

        explosion = GameController.Instance.asteroidExplosion;
        speed = GameController.Instance.EnemySpeed;
        child = transform.GetChild(0);

        float randomScale = Random.Range(0.95f, 1.05f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        randomRotation = Random.Range(-100, 100);
    }
    private void Update()
    {
        AsteroidMove();
        AsteroidRotate();
    }

    private void AsteroidMove()
    {
        transform.Translate(new Vector2(0, speed * Time.deltaTime));
    }

    private void AsteroidRotate()
    {
        child.rotation *= Quaternion.AngleAxis(randomRotation * Time.deltaTime, new Vector3(0, 0, 1));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("laser"))
        {
            GameController.Instance.score++;

            Destroy(collision.gameObject);           
            
            //explosion
            Instantiate(explosion, transform.position, Quaternion.identity);

            //loot
            if (Random.Range(0, 100) < 10) { Instantiate(GameController.Instance.coinPref, transform.position, Quaternion.identity); }

            //sound
            if (r.isVisible) { SoundController.Instance.EnemyExplosion(); }
                
            Destroy(gameObject);
        }
    }
}
