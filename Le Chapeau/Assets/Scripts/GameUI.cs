using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    // instance
    public static GameUI instance;

    void Awake()
    {
        // set the instance to this script
        instance = this;
    }

    void Start()
    {
        InitializePlayerUI();
    }

    void Update()
    {
        UpdatePlayerUI();
    }

    void InitializePlayerUI()
    {
        // loop through all containers
        for(int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUIContainer container = playerContainers[x];

            // only enable and modify the UI containers we need
            if(x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.timeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    void UpdatePlayerUI()
    {
        // loop through all players
        for(int x = 0; x < GameManager.instance.players.Length; ++x)
        {
            if(GameManager.instance.players[x] != null)
                playerContainers[x].hatTimeSlider.value = GameManager.instance.players[x].curHatTime;
        }
    }

    public void SetWinText (string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }
}

// class which holds info for each player's UI element
[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}
