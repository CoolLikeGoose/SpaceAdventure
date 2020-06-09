using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // TODO: delete this
    public static BossController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartBossBattle();
    }

    public void StartBossBattle()
    {
        GameController.Instance.isGameActive = false;

        Debug.Log("Boss battle started");
        StartCoroutine(firstEnter());
    }

    private IEnumerator firstEnter()
    {
        if (transform.position.y > 5)
        {
            transform.Translate(new Vector2(0, -0.005f));
            yield return null;
            StartCoroutine(firstEnter());
        }
        else
        {
            StartCoroutine(BossBattleProcess());
        }
    }

    private IEnumerator BossBattleProcess()
    {
        yield return new WaitForSeconds(5);

        Debug.Log("Boss battle ends");

        GameController.Instance.isGameActive = true;

        Destroy(gameObject);
    }
}
