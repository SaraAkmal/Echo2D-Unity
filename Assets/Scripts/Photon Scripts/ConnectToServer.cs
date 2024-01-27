using Photon.Pun;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
        PhotonNetwork.JoinOrCreateRoom("room_1", new RoomOptions(), new TypedLobby("lobby_1", LobbyType.Default));
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}