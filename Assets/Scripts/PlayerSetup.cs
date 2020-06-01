using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))]
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

    private GameObject playerUIInstance;
    Camera sceneCamera;
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
            //we are local player : disable scene camera
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            //disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLAyerName));

            //Create Player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            //no clone
            playerUIInstance.name = playerUIPrefab.name;

        }
        //player required component
        GetComponent<Player>().Setup();
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

        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }

    
}
