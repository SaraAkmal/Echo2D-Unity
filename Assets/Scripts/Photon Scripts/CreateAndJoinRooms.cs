using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;


    private void Start()
    {
        PhotonNetwork.JoinOrCreateRoom("ayaya", new RoomOptions(), new TypedLobby("nameee", LobbyType.Default));
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}