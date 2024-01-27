using System.Collections;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public enum GameStates
    {
        InsideMatch,
        EmptyRoom,
        EndMatch
    }

    public GameObject playerPrefab;

    [SerializeField] private GameObject rematchButton;
    [SerializeField] private GameObject leaveButton;

    public GameStates state;
    [SerializeField] private GameObject loadingFigure;
    [SerializeField] private AudioSource endAudio;
    [SerializeField] private Text leavingText;
    [SerializeField] private Text winLoseText;
    [SerializeField] private DebugManager debugManagerScript;

    private void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position,
            playerPrefab.transform.rotation);
        if (debugManagerScript.isOffline) // for quick debugging without reloading scenes and joining rooms
            state = GameStates.InsideMatch;
        else
            state = GameStates.EmptyRoom;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
        {
            ["IWantRematch"] = false
        });

        StartCoroutine(CheckMatch());
        if (PhotonNetwork.PlayerList.Length != 2)
        {
            winLoseText.text = "Waiting For Opponent";
            winLoseText.DOFade(1, 0.5f);
        }
    }

    public IEnumerator CheckMatch()
    {
        while (true)
        {
            if (state == GameStates.InsideMatch)
            {
                loadingFigure.gameObject.SetActive(false);
                winLoseText.DOFade(0, 0.2f).OnComplete(() =>
                {
                    winLoseText.color = new Color32(0xA8, 0x48, 0x42, 0xFF);
                    winLoseText.text = "Start!";
                    winLoseText.DOFade(1, 0.2f).OnComplete(() =>
                    {
                        winLoseText.DOFade(0, 1f).OnComplete(() => { winLoseText.text = ""; });
                    });
                });
                yield break;
            }

            yield return null;
        }
    }

    public IEnumerator CheckRematchLeaveBtn()
    {
        while (true)
        {
            if (PhotonNetwork.PlayerList.Length > 0)
                if ((bool) PhotonNetwork.PlayerList[0].CustomProperties["IWantRematch"] &&
                    (bool) PhotonNetwork.PlayerList[1].CustomProperties["IWantRematch"])
                {
                    ClearRematch();
                    photonView.RPC(nameof(LoadLevel), RpcTarget.All);
                    yield break;
                }

            yield return null;
        }
    }

    [PunRPC]
    private void EndMatchState()
    {
        endAudio.Play();
        rematchButton.SetActive(true);
        leaveButton.SetActive(true);
        state = GameStates.EndMatch;
    }

    public void ShowRematchOrLeaveBtns()
    {
        photonView.RPC(nameof(EndMatchState), RpcTarget.All);
        StartCoroutine(CheckRematchLeaveBtn());
    }

    private void ClearRematch()
    {
        for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            PhotonNetwork.PlayerList[i].CustomProperties["IWantRematch"] = false;
    }

    public void Rematch()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
        {
            ["IWantRematch"] = true
        });
        if (!((bool) PhotonNetwork.PlayerList[0].CustomProperties["IWantRematch"] &&
              !(bool) PhotonNetwork.PlayerList[1].CustomProperties["IWantRematch"]))
            photonView.RPC(nameof(PlayerWantsToRematchIcon), RpcTarget.All);
    }

    [PunRPC]
    private void PlayerWantsToRematchIcon()
    {
        loadingFigure.gameObject.SetActive(true);
    }

    [PunRPC]
    private void LoadLevel()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void Leave()
    {
        LeaveGame();
    }

    [PunRPC]
    private void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        rematchButton.SetActive(false);
        leaveButton.SetActive(false);
        loadingFigure.gameObject.SetActive(false);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        if (state != GameStates.EndMatch) endAudio.Play();
        var msgText = winLoseText;
        msgText.color = new Color32(0xA0, 0xA8, 0xA4, 0xFF);
        msgText.text = "Opponenet Left";
        leavingText.text = "Leaving in ";
        leavingText.DOFade(1, 0.7f);
        msgText.DOFade(1, 0.7f);
        StartCoroutine(Countdown(3));
        state = GameStates.EndMatch;

        IEnumerator Countdown(int time)
        {
            var wait = new WaitForSeconds(1);
            for (; time > 0; --time)
            {
                leavingText.text = "Leaving in " + time;
                yield return wait;
            }

            LeaveGame();
            yield return wait;
        }
    }
}