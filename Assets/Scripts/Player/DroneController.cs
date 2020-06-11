using System.Collections;
using UnityEngine;

/// <summary>
/// Manages player drones
/// </summary>
public class DroneController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WeaponShoot());
    }

    /// <summary>
    /// Coroutine that controls weapon shooting
    /// </summary>
    private IEnumerator WeaponShoot()
    {
        SoundController.Instance.LaserShot(0);

        Instantiate(GameController.Instance.weapons[4], new Vector2(transform.position.x, transform.position.y), Quaternion.identity);

        yield return new WaitForSeconds(GameController.Instance.playerShootDelay);
        StartCoroutine(WeaponShoot());

        yield return null;
    }
}
