using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    // TODO: delete this
    public static BossController Instance { get; private set; }

    //Boss mustnt take damage before firstEnter method doesnt end
    [NonSerialized] public bool isFighting = false;

    //controls the players health
    private float maxHp = 50;
    private float hp;
    [SerializeField] private Image healthBar;

    //Prefabs
    [SerializeField] private GameObject BossLaser;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hp = maxHp;
        StartBossBattle();
    }

    private void StartBossBattle()
    {
        GameController.Instance.isGameActive = false;

        Debug.Log("Boss battle started");
        StartCoroutine(firstEnter());
    }

    /// <summary>
    /// slow boss appearance
    /// </summary>
    private IEnumerator firstEnter()
    {
        if (transform.position.y > 5)
        {
            transform.Translate(new Vector2(UnityEngine.Random.Range(0.05f, -0.05f), -0.005f));
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, -0.3f, 0.3f), transform.position.y);
            yield return null;
            StartCoroutine(firstEnter());
        }
        else
        {
            //StartCoroutine(BossBattleProcess());
            isFighting = true;
            transform.position = new Vector2(0, 5);

            //Phase 1
            StartCoroutine(WeaponShoot());
        }
    }

    private IEnumerator BossBattleProcess()
    {
        return new WaitUntil(() => false);
    }

    /// <summary>
    /// Controls the correct completeion of the boss battle
    /// </summary>
    private IEnumerator BossBattleEnd()
    {
        isFighting = false;
        healthBar.fillAmount = 0;

        Debug.Log("Boss battle ends");

        StartCoroutine(explosionFX());
        yield return new WaitForSeconds(3f);

        isFighting = true;

        GameController.Instance.isGameActive = true;
        GameController.Instance.score += 20;

        Destroy(gameObject);
    }

    /// <summary>
    /// Creates explosion FX when boss is destroyed
    /// </summary>
    private IEnumerator explosionFX()
    {
        Vector2 pos = new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(3f, 5f));
        Instantiate(GameController.Instance.shipExplosion, pos, Quaternion.identity);

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(explosionFX());
    }

    private IEnumerator WeaponShoot()
    {
        Vector2 spawnLaser = new Vector2(0, 3);

        if (PlayerController.Instance == null) { yield break; }
        Vector2 targetPos = PlayerController.Instance.gameObject.transform.position; 
        targetPos -= spawnLaser;
        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg + 90;
        
        Instantiate(BossLaser, spawnLaser, Quaternion.AngleAxis(angle, Vector3.forward));

        yield return new WaitForSeconds(1f);
        StartCoroutine(WeaponShoot());
    }


    /// <summary>
    /// Reacts to hit from player shots
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("laser") && isFighting)
        {
            Destroy(collision.gameObject);

            hp--;

            if (hp <= 0) { StartCoroutine(BossBattleEnd()); }
            else { healthBar.fillAmount = hp / maxHp; }
        }
    }
}
