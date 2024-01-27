using DG.Tweening;
using Lean.Pool;
using Photon.Pun;
using UnityEngine;

public class FootprintsGenerator : MonoBehaviourPun
{
    [SerializeField] private GameObject footprintPrefab;
    public bool isHit;
    public bool isFootPrintHidden;

    private float generateTime;
    private FloatingJoystick joystick;
    private SpawnPlayers spawnPlayersScript;


    private void Start()
    {
        spawnPlayersScript = GameObject.Find("SpawnManager").GetComponent<SpawnPlayers>();
        joystick = GameObject.FindWithTag("Joystick").GetComponent<FloatingJoystick>();
    }

    private void Update()
    {
        if (joystick.Vertical != 0 || IsMoving()) generateTime += Time.deltaTime;

        if (generateTime < 0.5f || joystick.Vertical == 0 && !IsMoving() || !photonView.IsMine ||
            spawnPlayersScript.state == SpawnPlayers.GameStates.EndMatch) return;
        if (isHit)
        {
            photonView.RPC(nameof(SpawnFootprint), RpcTarget.All, GetFootprintRotationJoystick());
            if (isFootPrintHidden)
            {
                isFootPrintHidden = false;
                CancelInvoke(nameof(HideFootPrints));
                Invoke(nameof(HideFootPrints), 4);
            }
        }

        else
        {
            SpawnFootprint(GetFootprintRotationJoystick());
        }

        generateTime = 0;
    }

    private void HideFootPrints()
    {
        isHit = false;
    }

    [PunRPC]
    private void SpawnFootprint(Quaternion joystickRotation)
    {
        var footprintObj = LeanPool.Spawn(footprintPrefab, transform.position, joystickRotation);
        if (!photonView.IsMine)
            footprintObj.GetComponent<SpriteRenderer>().color = new Color32(145, 12, 12, 197);
        else
            footprintObj.GetComponent<SpriteRenderer>().color = Color.white;
        footprintObj.GetComponent<SpriteRenderer>().DOFade(1, .3f)
            .OnComplete(() =>
            {
                footprintObj.GetComponent<SpriteRenderer>().DOFade(0, .3f)
                    .SetDelay(1)
                    .OnComplete(() => { LeanPool.Despawn(footprintObj); });
            });
    }


    private Quaternion GetFootprintRotationJoystick()
    {
        var angle = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg - 90;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            angle = Mathf.Atan2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg - 90;
        var footprintRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        return footprintRotation;
    }

    private bool IsMoving()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            return true;
        return false;
    }
}