using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnManager;
    [SerializeField] private GameObject BulletObject;
    private PlayerMovement playerMovementScript;
    private TrailGeneration TrailGenerationScript;
    private float elapsedTime = 0;

    //to detect double click
    private const float doubleClickTime = 0.3f;

    private float lastClickTime;

    private void Start()
    {
        playerMovementScript = this.GetComponent<PlayerMovement>();
        TrailGenerationScript = spawnManager.GetComponent<TrailGeneration>();
    }

    private void Update()
    {
        DetectDoubleClick();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "xAxisWall" || collision.gameObject.name == "yAxisWall")
        {
            if (elapsedTime == 0)
            {
                TrailGenerationScript.TrailManagerStartEcho(collision, playerMovementScript.playerDirection, collision.GetContact(0).point);
                StartCoroutine(waitToSpawnEchoAgain());
            }
        }
        if (collision.gameObject.name == "Bullet")
        {
            //health decrease
        }
    }

    //wait four seconds if echo was generated
    private IEnumerator waitToSpawnEchoAgain()
    {
        float duration = 4f;
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
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= doubleClickTime)
            {
                GameObject spawnedBulletObject = LeanPool.Spawn(BulletObject, transform.position, Quaternion.identity);
                Vector3 direction = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                Vector3 playerPosition = transform.position;
                direction = direction - playerPosition;

                //spawnedBulletObject.GetComponent<BulletScript>().startDir = direction;
                spawnedBulletObject.GetComponent<BulletScript>().MoveBullet(direction);
                //fire bullet
            }
            else
            {
                // print("else one click");
            }
            lastClickTime = Time.time;
        }
    }
}