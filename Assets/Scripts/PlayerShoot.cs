using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";


    [SerializeField]
    private Camera camera;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private PlayerWeapon weapon;
    [SerializeField]
    private GameObject weaponGFX;
    [SerializeField]
    private string weaponLayerName = "Weapon";

    void Start()
    {
        if (camera == null)
        {
            Debug.Log("PlayerShoot : No camera referenced");
            this.enabled = false;
        }
        weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    //client - called only on client
    [Client]
    void Shoot()
    {
        RaycastHit _hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out _hit, weapon.range, mask))
        {
            //we hit something
            Debug.Log("We hit" + _hit.collider.name);
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, weapon.damage);
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
