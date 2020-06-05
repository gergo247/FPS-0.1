using UnityEngine;
using Mirror;
using System.Collections;
[RequireComponent(typeof(PlayerSetup))]
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

    [SyncVar]
    public string username = "Loading...";

    [SyncVar]
    public int kills;
    public int killsThisLife;
    [SyncVar]
    public int deaths;


    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;
    //audio
    AudioSource announcerAudio;
    //mutliple kill
    public AudioClip FirstBlood;
    public AudioClip DoubleKill;
    public AudioClip MultiKill;
    public AudioClip MegaKill;
    public AudioClip UltraKill;
    public AudioClip MonsterKill;
    public AudioClip LudicrousKill;
    public AudioClip HolyShit;
    // no die
    public AudioClip KillingSpree;
    public AudioClip Rampage;
    public AudioClip Dominating;
    public AudioClip Unstoppable;
    public AudioClip GODLIKE;
    public AudioClip WICKEDSICK;       

    #region rpc methods

    [ClientRpc]
    public void RpcTakeDamage(int _amount,string _sourceID)
    {
        if (isDead)
            return;
        currentHealt -= _amount;
        Debug.Log(transform.name + " now has " + currentHealt + " health.");

        if (currentHealt <= 0)
        {
            Die(_sourceID);
        }
    }
    #endregion rpc methods

    #region public methods

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            //switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        CmdBroadCastNewPlayerSetup();
    }
    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealt = maxHealth;
        //enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //enable gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //col nem behavious , nem lehet arraybe, itt enable
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }

        //create spawn effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        announcerAudio = GetComponent<AudioSource>();
        announcerAudio.volume = 0.2f;

    }
    #endregion public methods

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        //wait for position in clients before spawn avoid moving player before initing particles
        yield return new WaitForSeconds(0.1f);

        SetupPlayer();
        Debug.Log(transform.name + " respawned.");
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;
            sourcePlayer.killsThisLife++;
            sourcePlayer.PlayKillSound();
        }

        deaths++;
        killsThisLife = 0;

        //disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //disable gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        //disable the colider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }
        //spawn death effect Quaternion.identity - 0 0 0 rotation
        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
        //switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);

        }

        Debug.Log(transform.name + "is Dead");
        //call respawn
        StartCoroutine(Respawn());
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    RpcTakeDamage(9999);
        //}
        if (Input.GetKeyDown(KeyCode.K))
        {
            killsThisLife++;
            PlayKillSound();
        }
    }



    public void PlayKillSound()
    {
        if (announcerAudio == null)
            return;

            switch (killsThisLife)
        {
            case 1:
                announcerAudio.clip = FirstBlood;
                break;
            case 2:
                announcerAudio.clip = DoubleKill;
                break;
            case 3:
                announcerAudio.clip = MultiKill;
                break;
            case 4:
                announcerAudio.clip = MegaKill;
                break;
            case 5:
                announcerAudio.clip = UltraKill;
                break;
            case 6:
                announcerAudio.clip = MonsterKill;
                break;
            case 7:
                announcerAudio.clip = LudicrousKill;
                break;
            case 8:
                announcerAudio.clip = HolyShit;
                break;
          //  case 5:
          //      audio.clip = KillingSpree;
          //      break;
            case 10:
                announcerAudio.clip = Rampage;
                break;
            case 15:
                announcerAudio.clip = Dominating;
                break;
            case 20:
                announcerAudio.clip = Unstoppable;
                break;
            case 25:
                announcerAudio.clip = GODLIKE;
                break;
            case 30:
                announcerAudio.clip = WICKEDSICK;
                break;
            default:
                announcerAudio.clip = null;
                break;
        }
         announcerAudio.Play();
    }
}
