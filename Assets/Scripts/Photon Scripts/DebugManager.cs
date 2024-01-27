using Photon.Pun;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] public bool isOffline;
    [SerializeField] private MazeLoader mazeManager;

    private int randomSeedNum;

    private void Start()
    {
        if (isOffline)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRoom("roomname");
            randomSeedNum = Random.Range(1, 2000);
            mazeManager.InitializeMaze(randomSeedNum);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.LeaveRoom();
    }
}