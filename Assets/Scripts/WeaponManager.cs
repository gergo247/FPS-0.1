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

    void Start()
    {
        EquipWeapon(primaryWeapon);

       // weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);

    }
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject _weaponIns =  (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        //set parent so it follows movement
        _weaponIns.transform.SetParent(weaponHolder);
        if (isLocalPlayer)
            _weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);

    } 
}
