using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player super ability
/// </summary>
public class SuperAbilityController : MonoBehaviour
{
    public static SuperAbilityController Instance { get; private set; }

    private PlayerController player;

    [Tooltip("0 - drones, 1 - shield, 2 - fastShooting")]
    [SerializeField] private int abilityType;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        player = PlayerController.Instance;
    }

    public void ActivateAbility()
    {
        if (abilityType == 0) { }
        else if (abilityType == 1) { ActivateShield(); }
        else { }
    }

    public void DeactivateAbility()
    {
        if (abilityType == 0) { }
        else if (abilityType == 1) { DeactivateShield(); }
        else { }
    }

    //Drones


    //shields
    public void ActivateShield()
    {
        if (player.curShield != null) { Destroy(player.curShield.gameObject); }

        player.curShieldIndex = 3;
        GameObject shield = Instantiate(player.shieldsPrefs[player.curShieldIndex], transform.position, Quaternion.identity);
        shield.transform.SetParent(gameObject.transform);

        player.curShield = shield;
    }

    public void DeactivateShield()
    {
        Destroy(player.curShield.gameObject);

        player.curShieldIndex = -1;
    }

    //fastShooting
}
