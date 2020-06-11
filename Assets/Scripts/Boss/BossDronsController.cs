using System.Collections;
using UnityEngine;

public class BossDronsController : MonoBehaviour
{
    private int hp = 8;
    private GameObject Laser;

    private void Start()
    {
        Laser = BossController.Instance.DroneLaser;
        StartCoroutine(FirstEnter());
    }

    //Better perfomance then Update()
    private IEnumerator FirstEnter()
    {
        if (transform.parent.position.y > 5)
        {
            transform.parent.Translate(new Vector2(0, -1f * Time.deltaTime));
            yield return null;
            StartCoroutine(FirstEnter());
        }
        else
        {
            transform.parent.position = new Vector2(0, 5);

            StartCoroutine(WeaponShoot());
        }
    }

    /// <summary>
    /// Coroutine that controls weapon shooting
    /// </summary>
    private IEnumerator WeaponShoot()
    {
        Vector2 spawnLaser = transform.position;

        //Aims on player
        if (PlayerController.Instance == null) { yield break; }
        Vector2 targetPos = PlayerController.Instance.gameObject.transform.position;
        targetPos -= spawnLaser;
        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg + 90;

        //Laser shot in player position
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

            //when drone destroyed - boss get damage
            if (hp == 0)
            {
                BossController.Instance.DamageBoss(1);

                Instantiate(GameController.Instance.shipExplosion, transform.position, Quaternion.identity);
                SoundController.Instance.EnemyExplosion();

                Destroy(gameObject);
            }
        }
    }
}
