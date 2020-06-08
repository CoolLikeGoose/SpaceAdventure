using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates new enemy
/// </summary>
public class EnemyGenerator : MonoBehaviour
{
    //contains enemy prefabs
    [Tooltip("Asteroid - 0; Standard - 1, ")]
    [SerializeField] private GameObject[] enemyPrefs = null;

    private void Start()
    {
        StartCoroutine(SpawnAsteroid());
    }

    private IEnumerator SpawnAsteroid()
    {
        GameObject toSpawn = enemyPrefs[0];

        //20% to spawn enemy instead of asteroid
        if (Random.Range(0, 100) < 20) { toSpawn = enemyPrefs[Random.Range(1, enemyPrefs.Length)]; }

        Vector2 spawnPos = new Vector2(Random.Range(-2.3f, 2.3f), transform.position.y);
        Instantiate(toSpawn, spawnPos, Quaternion.identity);

        if (GameController.Instance.isGameActive)
        {
            yield return new WaitForSeconds(GameController.Instance.enemySpawnDelay);
            StartCoroutine(SpawnAsteroid());
        }

        yield return null;
    }
}
