using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPun
{
    //to detect double click
    private const float doubleClickTime = 0.3f;
    [SerializeField] private GameObject BulletObject;
    [SerializeField] private float offset;
    [SerializeField] private GameObject bloodStain;
    [SerializeField] private AudioClip emptyGunClip;
    [SerializeField] private AudioClip reloadGunClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip bloodStainClip;

    private int ammo = 3;

    private float elapsedTime;

    private int health = 4;
    private Image healthBar;

    private bool isPosChanged;
    private float lastClickTime;
    private PlayerMovement playerMovementScript;


    private List<Vector2> playerPositions;
    private AudioSource[] sounds;
    private GameObject spawnManager;
    private SpawnPlayers spawnPlayersScript;
    private TrailGeneration TrailGenerationScript;


    private void Start()
    {
        isPosChanged = false;
        spawnManager = GameObject.Find("SpawnManager");
        playerMovementScript = GetComponent<PlayerMovement>();
        TrailGenerationScript = spawnManager.GetComponent<TrailGeneration>();
        spawnPlayersScript = spawnManager.GetComponent<SpawnPlayers>();
        healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
        sounds = gameObject.GetComponents<AudioSource>();
        StartCoroutine(ChangePlayerPosition());
    }


    private void Update()
    {
        DetectDoubleClick();
        if (photonView.IsMine && spawnPlayersScript.state != SpawnPlayers.GameStates.EndMatch)
            if (Input.GetKeyDown(KeyCode.Space))
                if (elapsedTime == 0)
                    photonView.RPC(nameof(JumpEffect), RpcTarget.All, (Vector2) transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TrailGenerationScript != null)
            if ((collision.gameObject.name == "xAxisWall" || collision.gameObject.name == "yAxisWall") &&
                spawnPlayersScript.state == SpawnPlayers.GameStates.InsideMatch)
                if (elapsedTime == 0)
                {
                    GetComponent<FootprintsGenerator>().isHit = true;
                    GetComponent<FootprintsGenerator>().isFootPrintHidden = true;
                    collision.gameObject.GetComponent<AudioSource>().pitch = Random.Range(1f, 3f);
                    collision.gameObject.GetComponent<AudioSource>().Play();
                    TrailGenerationScript.TrailManagerStartEcho(collision, playerMovementScript.playerDirection,
                        collision.GetContact(0).point);
                    StartCoroutine(WaitToSpawnEchoAgain(1));
                }

        if (spawnPlayersScript != null)
            if (photonView.IsMine != null && spawnPlayersScript.state != null)
                if (photonView.IsMine && spawnPlayersScript.state != SpawnPlayers.GameStates.EndMatch)
                    if (collision.gameObject.name == "Bullet")
                    {
                        photonView.RPC(nameof(SpawnBloodStain), RpcTarget.All, (Vector2) transform.position,
                            transform.rotation);
                        health -= 1;
                        healthBar.fillAmount -= 0.25f;
                        GetComponent<FootprintsGenerator>().isHit = true;
                        if (health == 0)
                        {
                            var msgText = GameObject.FindWithTag("WinLoseText").GetComponent<Text>();
                            msgText.color = new Color32(0xA8, 0x48, 0x42, 0xFF);
                            msgText.text = PhotonNetwork.LocalPlayer.NickName + " Lost";
                            msgText.DOFade(1, 0.7f);
                            print(msgText.text);
                            photonView.RPC(nameof(SetWinLoseText), RpcTarget.Others);
                            spawnPlayersScript.ShowRematchOrLeaveBtns();
                        }
                    }
    }

    [PunRPC]
    private void JumpEffect(Vector2 pos)
    {
        GetComponent<FootprintsGenerator>().isHit = true;
        GetComponent<FootprintsGenerator>().isFootPrintHidden = true;
        TrailGenerationScript.TrailManagerJumpEcho(
            pos);
        sounds[1].clip = jumpClip;
        sounds[1].pitch = Random.Range(1f, 3f);
        sounds[1].Play();
        StartCoroutine(WaitToSpawnEchoAgain(4));
    }

    [PunRPC]
    private void SetWinLoseText()
    {
        var msgText = GameObject.FindWithTag("WinLoseText").GetComponent<Text>();
        msgText.color = Color.green;
        msgText.text = PhotonNetwork.LocalPlayer.NickName + " Won";
        msgText.DOFade(1, 0.7f);
    }


    [PunRPC]
    private void SpawnBloodStain(Vector2 pos, Quaternion rotation)
    {
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        var stain = LeanPool.Spawn(bloodStain, pos, rotation);
        sounds[1].clip = bloodStainClip;
        sounds[1].pitch = Random.Range(1f, 3f);
        sounds[1].Play();
        stain.GetComponent<SpriteRenderer>().DOFade(1, .2f)
            .OnComplete(() =>
            {
                stain.GetComponent<SpriteRenderer>().DOFade(0, .3f)
                    .SetDelay(1)
                    .OnComplete(() => { LeanPool.Despawn(stain); });
            });
    }

    private IEnumerator ChangePlayerPosition() //todo
    {
        while (true)
        {
            if (isPosChanged)
            {
                spawnPlayersScript.state = SpawnPlayers.GameStates.InsideMatch;

                yield break;
            }

            if (PhotonNetwork.CurrentRoom.Players.Count == 2)
            {
                playerPositions = spawnManager.GetComponent<MazeLoader>().CalculateCenterPoints();
                // if (photonView.Owner.ActorNumber > 2) //todo
                //     transform.position = playerPositions[Random.Range(0, 16)];
                // else
                var randomIndex = photonView.Owner.ActorNumber + Random.Range(0, 11);
                transform.position = playerPositions[randomIndex];
                isPosChanged = true;
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }
    }


    //wait four seconds if echo was generated
    private IEnumerator WaitToSpawnEchoAgain(int duration)
    {
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
    }

    private void DetectDoubleClick()
    {
        if (photonView.IsMine && Input.GetMouseButtonDown(0) &&
            spawnPlayersScript.state != SpawnPlayers.GameStates.EndMatch)
        {
            var timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= doubleClickTime && ammo > 0)
            {
                var direction = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                var playerPosition = (Vector2) transform.position;
                direction = direction - playerPosition;
                ammo--;

                photonView.RPC(nameof(SpawnAndFireBullet), RpcTarget.All, direction);
            }
            else if (ammo == 0)
            {
                sounds[1].clip = emptyGunClip;
                sounds[1].Play();
            }

            lastClickTime = Time.time;
        }

        if (Input.GetKey(KeyCode.R) && ammo == 0)
        {
            ammo = 3;
            sounds[1].clip = reloadGunClip;
            sounds[1].Play();
        }
    }


    [PunRPC]
    private void SpawnAndFireBullet(Vector2 direction)
    {
        var offsetDirection = direction.normalized * offset;
        var startPosition = (Vector2) transform.position + offsetDirection;
        var spawnedBulletObject = LeanPool.Spawn(BulletObject, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), spawnedBulletObject.GetComponent<Collider2D>(),
            true);
        spawnedBulletObject.GetComponent<BulletScript>()
            .MoveBullet(direction, gameObject.GetComponent<Collider2D>()); //fire bullet
    }
}