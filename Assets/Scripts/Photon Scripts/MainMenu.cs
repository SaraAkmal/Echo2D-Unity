using System.Collections;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    private const string GameVersion = "0.1";
    private const int maxPlayersPerRoom = 2;
    [SerializeField] private Text waitingStatusPanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField createRoomInput;
    [SerializeField] private AudioSource bgMusic;
    private bool isRandomRoom;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void FindOpponent()
    {
        waitingStatusPanel.DOFade(0, 0);
        if (nameInput.text == "")
        {
            waitingStatusPanel.text = "Enter Your Name";
            waitingStatusPanel.DOFade(1, 0.7f);
            return;
        }

        if (createRoomInput.text == "")
        {
            waitingStatusPanel.text = "Joining Random Room...";
            isRandomRoom = true;
        }
        else
        {
            waitingStatusPanel.text = "Joining Room...";
            isRandomRoom = false;
        }

        waitingStatusPanel.DOFade(1, 0.7f);
        StartCoroutine(FindOpponentIE());

        IEnumerator FindOpponentIE()
        {
            while (!PhotonNetwork.IsConnectedAndReady) yield return null;
            PhotonNetwork.JoinLobby();
        }
    }

    public void CreateRoomBtn()
    {
        waitingStatusPanel.DOFade(0, 0f);
        if (createRoomInput.text == "")
        {
            waitingStatusPanel.text = "Enter Room Name";
        }
        else if (nameInput.text == "")
        {
            waitingStatusPanel.text = "Enter Your Name";
        }
        else
        {
            PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
            FindOpponent();
        }

        waitingStatusPanel.DOFade(1, 0.7f);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for an opponent,creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        waitingStatusPanel.text = "Room doesnt exist";
    }

    public override void OnJoinedLobby()
    {
        print("Joined Lobby ");
        if (!isRandomRoom)
            PhotonNetwork.JoinRoom(createRoomInput.text);
        else
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        bgMusic.DOFade(0, 1f).OnComplete(() =>
        {
            Debug.Log("Client Successfuly joined a room");
            PhotonNetwork.LoadLevel("GameScene");
            PhotonNetwork.NickName = nameInput.text;
        });
    }
}