using System;
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

    private int weaponIndex;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        player = PlayerController.Instance;
        GUIController.Instance.reloadSpriteSet = abilityType;
    }

    public void ActivateAbility()
    {
        if (abilityType == 0) { ActivateDrones(); }
        else if (abilityType == 1) { ActivateShield(); }
        else { ActivateFastShooting(); }
    }

    public void DeactivateAbility()
    {
        if (abilityType == 0) { DeactivateDrones(); }
        else if (abilityType == 1) { DeactivateShield(); }
        else { DeactivateFastShooting(); }
    }

    //Drones

    private void ActivateDrones()
    {
        GameObject drone = Instantiate(GameController.Instance.dronePref, transform.position, Quaternion.identity);
        drone.transform.SetParent(gameObject.transform);

        weaponIndex = player.curWeaponIndex;
        player.curWeaponIndex = 7;    
    }

    private void DeactivateDrones()
    {
        Destroy(transform.GetComponentInChildren<DroneController>().gameObject.transform.parent.gameObject);

        player.curWeaponIndex = weaponIndex;
    }

    //shields
    private void ActivateShield()
    {
        if (player.curShield != null) { Destroy(player.curShield.gameObject); }

        player.curShieldIndex = 3;
        GameObject shield = Instantiate(player.shieldsPrefs[player.curShieldIndex], transform.position, Quaternion.identity);
        shield.transform.SetParent(gameObject.transform);

        player.curShield = shield;
    }

    private void DeactivateShield()
    {
        Destroy(player.curShield.gameObject);

        player.curShieldIndex = -1;
    }

    //fastShooting
    private void ActivateFastShooting()
    {
        weaponIndex = player.curWeaponIndex;
        player.curWeaponIndex = 6;
        GameController.Instance.playerShootDelay /= 3;
    }

    private void DeactivateFastShooting()
    {
        player.curWeaponIndex = weaponIndex;
        GameController.Instance.playerShootDelay *= 3;
    }
}
