using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    string remoteLayerName = "RemotePlayer";
    [SerializeField]
    string dontDrawLAyerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;
    [SerializeField]
    GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;
    void Start()
    {
        //if we dont control the player disable components
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLAyerName));

            //Create Player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            //no clone
            playerUIInstance.name = playerUIPrefab.name;
            //configure player ui
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No playerUI component on PlayerUI prefab.");
            ui.SetController(GetComponent<PlayerController>());

            //player required component
            GetComponent<Player>().SetupPlayer();
            string _username = PlayerPrefs.GetString("playerName");
            if (string.IsNullOrEmpty(_username))
            {
                 _username = transform.name;
            }

            CmdSetUsername(transform.name, _username);
        }

    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
       Player player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            Debug.Log(username + " has joined");
            player.username = username;
        }
    }

    void SetLayerRecursively(GameObject gameObject, int newLayer)
    {
        gameObject.layer = newLayer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject,newLayer);
        }
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        //nemkell null check, required type
        GameManager.RegisterPlayer(_netId, _player);
    }
    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }
    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    //on object disable or destroy
    void OnDisable()
    {
        Destroy(playerUIInstance);
        if (isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true);
        GameManager.UnRegisterPlayer(transform.name);
    }

    
}
