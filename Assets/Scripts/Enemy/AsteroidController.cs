﻿using UnityEngine;

/// <summary>
/// Manages Asteroid movement/rotation/destruction
/// </summary>
public class AsteroidController : MonoBehaviour
{
    private float speed;
    private Transform child;

    private float randomRotation;
    float randomScale;

    private GameObject explosion;
    private GameObject damage;

    private Renderer r;

    private int hp;

    private void Start()
    {
        //if object is inVisible sounds will not play
        r = GetComponentInChildren<Renderer>();

        explosion = GameController.Instance.asteroidExplosion;
        damage = GameController.Instance.asteroidHitParticles;

        speed = GameController.Instance.EnemySpeed;
        child = transform.GetChild(0);

        randomScale = Random.Range(0.7f, 1.3f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        //hp depends from scale
        if (randomScale < 0.9f) { hp = 1; }
        else if (randomScale < 1.1f) { hp = 2; }
        else { hp = 3; }

        randomRotation = Random.Range(-100, 100);
    }
    private void Update()
    {
        AsteroidMove();
        AsteroidRotate();
    }

    private void AsteroidMove()
    {
        transform.Translate(new Vector2(0, speed * Time.deltaTime * GameController.Instance.gameSpeed));
    }

    //rotate asteroid Sprite
    private void AsteroidRotate()
    {
        child.rotation *= Quaternion.AngleAxis(randomRotation * Time.deltaTime, new Vector3(0, 0, 1));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("laser"))
        {
            Destroy(collision.gameObject);

            DamageAsteroid();
        }
    }

    /// <summary>
    /// Damage the asteroid and Instantiate hit particles
    /// </summary>
    private void DamageAsteroid()
    {
        hp -= GameController.Instance.playerDamage;
        if (hp <= 0) { DestroyAsteroid(); }

        //Damage particles
        Instantiate(damage, transform.position, Quaternion.identity);
    }

    //called from PlayerController, when shield collide with asteroid
    /// <summary>
    /// Destroy asteroid process (score, loot, explosion, sound)
    /// </summary>
    public void DestroyAsteroid()
    {
        GameController.Instance.score++;

        //explosion
        GameObject explosionFX = Instantiate(explosion, transform.position, Quaternion.identity);
        explosionFX.transform.localScale = new Vector3(randomScale, randomScale, randomScale);


        //loot 25%
        if (Random.Range(0, 100) < 20) { Instantiate(GameController.Instance.coinPref, transform.position, Quaternion.identity); }
        //chance (70*7) ~ 5%
        else if (Random.Range(0, 100) < 6) { Instantiate(GameController.Instance.shieldPref, transform.position, Quaternion.identity); }

        //sound
        if (r.isVisible) { SoundController.Instance.EnemyExplosion(); }

        Destroy(gameObject);
    }
}
