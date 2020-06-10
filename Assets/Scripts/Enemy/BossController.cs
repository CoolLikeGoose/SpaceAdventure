using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    // TODO: delete this
    public static BossController Instance { get; private set; }

    [NonSerialized] public bool isFighting = false;

    private float maxHp = 10;
    private float hp;
    [SerializeField] private Image healthBar;

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
        }
    }

    private IEnumerator BossBattleProcess()
    {
        return new WaitUntil(() => false);
    }

    private IEnumerator BossBattleEnd()
    {
        isFighting = false;
        healthBar.fillAmount = 0;

        Debug.Log("Boss battle ends");

        StartCoroutine(explosionFX());
        yield return new WaitForSeconds(3f);
        //player Coroutine WeaponShoot should continue
        isFighting = true;
        yield return null;

        GameController.Instance.isGameActive = true;
        GameController.Instance.score += 20;

        Destroy(gameObject);
    }


    private IEnumerator explosionFX()
    {
        Vector2 pos = new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(3f, 5f));
        Instantiate(GameController.Instance.shipExplosion, pos, Quaternion.identity);

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(explosionFX());
    }

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
