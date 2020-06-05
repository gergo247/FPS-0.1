using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    GameObject playerScoreBoardItem;
    [SerializeField]
    Transform playerScoreboardList;

    void OnEnable()
    {
        //Get an array of players
        Player[] players = GameManager.GetAllPlayers();
        //loop through and set up a list item for each one 
        //setting the ui elements equal to the relevant data
        foreach (Player player in players)
        {
           GameObject itemGO =  (GameObject)Instantiate(playerScoreBoardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if (item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
    }

    void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
        //clean up our list of items
    }
}
