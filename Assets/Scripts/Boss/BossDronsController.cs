using System.Collections;
using UnityEngine;

public class BossDronsController : MonoBehaviour
{
    private int hp = 5;
    private GameObject Laser;

    private void Start()
    {
        Laser = BossController.Instance.DroneLaser;
        StartCoroutine(WeaponShoot());
    }

    /// <summary>
    /// Coroutine that controls weapon shooting
    /// </summary>
    private IEnumerator WeaponShoot()
    {
        Vector2 spawnLaser = transform.position;

        if (PlayerController.Instance == null) { yield break; }
        Vector2 targetPos = PlayerController.Instance.gameObject.transform.position;
        targetPos -= spawnLaser;
        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg + 90;

        Instantiate(Laser, spawnLaser, Quaternion.AngleAxis(angle, Vector3.forward));

        yield return new WaitForSeconds(2f);
        StartCoroutine(WeaponShoot());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("laser"))
        {
            hp--;
            Instantiate(GameController.Instance.shipHitParticles, transform.position, Quaternion.identity);

            Destroy(collision.gameObject);
            if (hp == 0)
            {
                BossController.Instance.DamageBoss(1);
                Destroy(gameObject);
            }
        }
    }
}
