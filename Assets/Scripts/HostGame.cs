using UnityEngine;
using Mirror;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 16;

    private string roomName;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;

    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }


    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating a Room: " + roomName + " with room for " + roomSize + " players.");
            //create room
        }
    }
}
