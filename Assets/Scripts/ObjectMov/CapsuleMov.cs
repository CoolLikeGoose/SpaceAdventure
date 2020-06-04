using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that cotains data about weapon and number of bullets
/// </summary>
public class CapsuleMov : MonoBehaviour
{
    public int carryWeaponIndex;
    public int amountBullets;

    private float speed;
    private void Start()
    {
        speed = GameController.Instance.EnemySpeed;
    }
    void Update()
    {
        transform.Translate(new Vector2(0, speed * Time.deltaTime));
    }
}
