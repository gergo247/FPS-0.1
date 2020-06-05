using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";


    [SerializeField]
    private Camera camera;
    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    void Start()
    {
        if (camera == null)
        {
            Debug.Log("PlayerShoot : No camera referenced");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }
    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        //single frie
        if (currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }

    }
  
    //called on a server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }
    //is called on the server when we hit something
    //takes in the hit point and normal of the surface we hit
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoDoHitEffect(_pos, _normal);
    }
    //is called on all clients, here we can spawn effects
    [ClientRpc]
    void RpcDoDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        //may be resource hungry, object-pooling might be solution
       GameObject _hiteffect =  (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));

        Destroy(_hiteffect, 2f);
    }
    //called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }
    //client - called only on client
    [Client]
    void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //only local player
        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;
        Debug.Log("Remaining bullets :" +currentWeapon.bullets);

        //we are shooting, call the onshoot method on server
        CmdOnShoot();
        RaycastHit _hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out _hit, currentWeapon.range, mask))
        {
            //we hit something
            //Debug.Log("We hit" + _hit.collider.name);
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }
            //we hit something, call the OnHit method on the server
            CmdOnHit(_hit.point, _hit.normal);
        }
    }
    //command - called only on server
    [Command]
    void CmdPlayerShot(string _playerID, int _damage,string _sourceID)
    {
        Debug.Log(_playerID + " has been shot");
        //slow, stb
        //Destroy(GameObject.Find(_ID));
       Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourceID);
    }
}
