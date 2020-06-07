using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WeaponShoot());
    }

    private IEnumerator WeaponShoot()
    {
        SoundController.Instance.LaserShot(0);

        Instantiate(GameController.Instance.weapons[4], new Vector2(transform.position.x, transform.position.y), Quaternion.identity);

        if (GameController.Instance.isGameActive)
        {
            yield return new WaitForSeconds(GameController.Instance.playerShootDelay);
            StartCoroutine(WeaponShoot());
        }

        yield return null;
    }
}
