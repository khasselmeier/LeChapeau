using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;       // has the game ended?
    public float timeToWin;              // time a player needs to hold the hat to win
    public float invincibleDuration;     // how long after a player gets the hat, are they invincible
    private float hatPickupTime;         // the time the hat was picked up by the current player

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerControllerScript[] players;
    public int playerWithHat;
    public int playersInGame;

    // instance
    public static GameManager instance;

    private void Awake()
    {
        // instance
        instance = this;
    }

    private void Start()
    {
        players = new PlayerControllerScript[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        // when all of the players are in the game -- spawn them
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    // spawns a player an initializes it
    void SpawnPlayer()
    {
        // instances the player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // get the player script
        PlayerControllerScript playerScript = playerObj.GetComponent<PlayerControllerScript>();

        // initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerControllerScript GetPlayer (int playerID)
    {
        return players.First(x => x.id == playerID);
    }

    public PlayerControllerScript GetPlayer (GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    // called when the player hits the currently hatted player - giving them the hat
    [PunRPC]
    public void GiveHat (int playerID, bool initialGive)
    {
        // remove the hat from the current hatted player
        if(!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        // gives the hat to the new player
        playerWithHat = playerID;
        GetPlayer(playerID).SetHat(true);
        hatPickupTime = Time.time;
    }

    // is the player able to take the hat at this current time (is the other player invincible)?
    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame (int playerID)
    {
        gameEnded = true;
        PlayerControllerScript player = GetPlayer(playerID);

        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
