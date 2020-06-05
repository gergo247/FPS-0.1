using UnityEngine;
using Mirror;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    private bool isReloading = false;
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
    public WeaponGraphics GetCurrentGraphics()
    {
        Debug.Log("GetCurrentGraphics:" + currentGraphics.name);
        return currentGraphics;
    }
    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);


        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        Debug.Log("currentGraphics:" + currentGraphics.name);
        if (currentGraphics == null)
        {
            Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);
        }

        //set parent so it follows movement
        _weaponIns.transform.SetParent(weaponHolder);
        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));

    }

    public void Reload()
    {
        if (isReloading)
            return;

        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine()
    {
        isReloading = true;

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }
}
