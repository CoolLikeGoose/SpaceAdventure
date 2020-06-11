using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    // TODO: delete this
    public static BossController Instance { get; private set; }

    //Boss mustnt take damage before firstEnter method doesnt end
    private bool isFighting = false;

    private Coroutine currentPhase;

    //controls the players health
    private float maxHp = 100;
    private float hp;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private GameObject shieldBar = null;

    //Prefabs
    [SerializeField] private GameObject BossLaser = null;
    public GameObject DroneLaser;
    [SerializeField] private GameObject Drones = null;
    [SerializeField] private GameObject Asteroid = null;

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

            //Start battle
            StartCoroutine(BossBattleProcess());
        }
    }

    private IEnumerator BossBattleProcess()
    {
        //1 phase
        currentPhase = StartCoroutine(FirstPhase());
        yield return new WaitUntil(() => hp < maxHp / 10 * 6.6);
        StopCoroutine(currentPhase);

        //2 phase
        currentPhase = StartCoroutine(SecondPhase());
        yield return new WaitUntil(() => hp < maxHp / 10 * 3.3);
        StopCoroutine(currentPhase);

        //3 phase
        currentPhase = StartCoroutine(ThirdPhase());
        yield return new WaitUntil(() => hp <= 0);
        StopCoroutine(currentPhase);
    }

    /// <summary>
    /// Controls the correct completeion of the boss battle
    /// </summary>
    private IEnumerator BossBattleEnd()
    {
        isFighting = false;
        healthBar.fillAmount = 0;

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

    //
    //PHASES
    //
    private IEnumerator FirstPhase()
    {
        Vector2 spawnLaser = new Vector2(0, 3);

        if (PlayerController.Instance == null) { yield break; }
        Vector2 targetPos = PlayerController.Instance.gameObject.transform.position;
        targetPos -= spawnLaser;
        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg + 90;

        Instantiate(BossLaser, spawnLaser, Quaternion.AngleAxis(angle, Vector3.forward));

        yield return new WaitForSeconds(1f);
        currentPhase = StartCoroutine(FirstPhase());
    }

    private IEnumerator SecondPhase()
    {
        GameObject drones = Instantiate(Drones, transform.position, Quaternion.identity);

        shieldBar.SetActive(true);
        isFighting = false;

        yield return new WaitUntil(() => drones.transform.childCount == 0);

        shieldBar.SetActive(false);
        isFighting = true;
    }

    private IEnumerator ThirdPhase()
    {
        Vector2 spawn = new Vector2(0, 3); 

        if (PlayerController.Instance == null) { yield break; }
        Vector2 targetPos = PlayerController.Instance.gameObject.transform.position;
        targetPos -= spawn;
        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg + 90;

        Instantiate(Asteroid, spawn, Quaternion.AngleAxis(angle, Vector3.forward));

        yield return new WaitForSeconds(1f);
        currentPhase = StartCoroutine(ThirdPhase());
    }


    /// <summary>
    /// Reacts to hit from player shots
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("laser") && isFighting)
        {
            Destroy(collision.gameObject);

            if (hp <= 0) { StartCoroutine(BossBattleEnd()); }
            else { DamageBoss(0); }
        }
    }

    /// <summary>
    /// Deals damage to the boss and displays its current hp on healthBar
    /// </summary>
    /// <param name="damageState">0 - damage from laser, 1 - damage from destroyed drone</param>
    public void DamageBoss(int damageState)
    {
        if (damageState == 0) { hp -= 1; }
        else { hp -= maxHp / 3 / 4; }

        healthBar.fillAmount = hp / maxHp;
    }
}
