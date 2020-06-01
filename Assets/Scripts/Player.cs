using UnityEngine;
using Mirror;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    [SyncVar]
    private int currentHealt;


    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;


    #region rpc methods

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
            return;
        currentHealt -= _amount;
        Debug.Log(transform.name + " now has " + currentHealt + " health.");

        if (currentHealt <= 0)
        {
            Die();
        }
    }
    #endregion rpc methods

    #region public methods
   
    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }
    public void SetDefaults()
    {
        isDead = false;
        currentHealt = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //col nem behavious , nem lehet arraybe, itt enable
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }
    }
    #endregion public methods
    //void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(9999);
    //    }
    //}
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;


        Debug.Log(transform.name + " respawned.");
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        Debug.Log(transform.name + "is Dead");
        //call respawn
        StartCoroutine(Respawn());
    }
}
