using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates new enemy
/// </summary>
public class EnemyGenerator : MonoBehaviour
{
    //contains enemy prefabs
    [SerializeField] private GameObject asteroidPref = null;
    [SerializeField] private GameObject enemyPref = null;

    private void Start()
    {
        StartCoroutine(SpawnAsteroid());
    }

    private IEnumerator SpawnAsteroid()
    {
        GameObject toSpawn = asteroidPref;

        if (Random.Range(0, 100) < 20) { toSpawn = enemyPref; }

        Vector2 spawnPos = new Vector2(Random.Range(-2.4f, 2.4f), transform.position.y);
        Instantiate(toSpawn, spawnPos, Quaternion.identity);

        if (GameController.Instance.isGameActive)
        {
            yield return new WaitForSeconds(GameController.Instance.enemySpawnDelay);
            StartCoroutine(SpawnAsteroid());
        }

        yield return null;
    }
}
