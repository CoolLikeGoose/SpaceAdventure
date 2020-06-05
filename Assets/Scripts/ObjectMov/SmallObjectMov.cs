using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves things like bullets(enemy, player) and coins
/// </summary>
public class SmallObjectMov : MonoBehaviour
{
    [Range(0, 2)] [Tooltip("0 - player; 1 - enemy; 2 - coin")]
    [SerializeField] private int sender = 0;
    [SerializeField] private bool hasChilds = false;

    private float speed;

    private void Start()
    {
        if (sender == 0) { speed = GameController.Instance.playerLaserSpeed; }
        else if (sender == 1) { speed = GameController.Instance.enemyLaserSpeed; }
        else { speed = GameController.Instance.EnemySpeed; }
    }
    void Update()
    {
        transform.Translate(new Vector2(0, speed * Time.deltaTime * GameController.Instance.gameSpeed));
    }
    private void LateUpdate()
    {
        if (hasChilds && transform.childCount == 0 || transform.position.y < -15) { Destroy(gameObject); }
    }
}
