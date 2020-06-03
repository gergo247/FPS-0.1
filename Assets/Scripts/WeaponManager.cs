using UnityEngine;
using Mirror;

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
        GameObject _weaponIns =  (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);


        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        Debug.Log("currentGraphics:" + currentGraphics.name);
        if (currentGraphics == null)
        {
            Debug.LogError("No WeaponGraphics component on the weapon object: "+ _weaponIns.name);
        }

        //set parent so it follows movement
        _weaponIns.transform.SetParent(weaponHolder);
        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));

    } 
}
