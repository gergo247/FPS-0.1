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
                InvokeRepeating("Shoot",0f,1f/currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
       
    }
    //client - called only on client
    [Client]
    void Shoot()
    {
        Debug.Log("Shoot!");

        RaycastHit _hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out _hit, currentWeapon.range, mask))
        {
            //we hit something
            Debug.Log("We hit" + _hit.collider.name);
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
        }
    }

    //command - called only on server
    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " has been shot");
        //slow, stb
        //Destroy(GameObject.Find(_ID));
       Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
