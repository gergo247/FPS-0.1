using UnityEngine;
using Mirror;
using System.Collections;
using System;

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
    [HideInInspector]
    public bool isReloading = false;

    //audio
    AudioSource reloadAudio;
    //shoot sound effects
    public AudioClip reloadSound;

    void Start()
    {
        EquipWeapon(primaryWeapon);
        reloadAudio = GetComponent<AudioSource>();
        reloadAudio.volume = 0.2f;
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
        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }
    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }
    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currentGraphics.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Reload");
        }

        if (reloadAudio != null)
            reloadAudio.PlayOneShot(reloadSound);

    }
    
}
