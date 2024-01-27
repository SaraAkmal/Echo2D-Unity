using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    private AudioSource footStepAudio;

    private bool isWalking;

    private FloatingJoystick joystick;
    private PhotonView photonView;
    private Rigidbody2D playerRigidBody;
    private GameObject spawnManager;
    private SpawnPlayers spawnPlayersScript;
    public Vector2 playerDirection { get; private set; } //direction

    public bool IsWalking
    {
        get => isWalking;
        set
        {
            if (value == isWalking)
                return;

            isWalking = value;
            if (isWalking)
                footStepAudio.DOFade(0.9f, 0.2f);

            else
                footStepAudio.DOFade(0, 0.2f);
        }
    }

    private void Start()
    {
        spawnManager = GameObject.Find("SpawnManager");
        spawnPlayersScript = spawnManager.GetComponent<SpawnPlayers>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        joystick = GameObject.FindWithTag("Joystick").GetComponent<FloatingJoystick>();
        footStepAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (photonView.IsMine && spawnPlayersScript.state != SpawnPlayers.GameStates.EndMatch)
        {
            if (joystick.Vertical != 0)
            {
                IsWalking = true;
                playerRigidBody.velocity = new Vector2(joystick.Horizontal * playerSpeed,
                    joystick.Vertical * playerSpeed);
                playerDirection = playerRigidBody.velocity.normalized;
            }
            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                IsWalking = true;
                playerRigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * playerSpeed,
                    Input.GetAxis("Vertical") * playerSpeed);
                playerDirection = playerRigidBody.velocity.normalized;
            }
            else
            {
                IsWalking = false;
                playerRigidBody.velocity = Vector2.zero;
            }
        }
        else if (spawnPlayersScript.state == SpawnPlayers.GameStates.EndMatch)
        {
            IsWalking = false;
            playerRigidBody.velocity = Vector2.zero;
        }
    }
}