//using System.Collections;
//using UnityEngine;
//[RequireComponent(typeof(Player))]
//public class PlayerScore : MonoBehaviour
//{
//    int lastKills = 0;
//    int lastDeaths = 0;

//    Player player;
//    // Start is called before the first frame update
//    void Start()
//    {
//        player = GetComponent<Player>();
//        StartCoroutine(SyncScoreLoop());
//    }
//    void OnDestroy()
//    {
//        if (player != null)
//            SyncNow();
//    }
//    IEnumerator SyncScoreLoop()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(5f);
//            SyncNow();
//        }
//    }
//    void SyncNow()
//    {
//        OnDataRevieved();
//    }
//    void OnDataRevieved()
//    {
//        int killsSinceLast = player.kills - lastKills;
//        int deathsSinceLast = player.deaths - lastDeaths;

//        if (killsSinceLast == 0 && deathsSinceLast == 0)
//            return;
//    }
//}
