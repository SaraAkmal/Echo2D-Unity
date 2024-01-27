using System.Collections;
using Lean.Pool;
using Photon.Pun;
using UnityEngine;

public class BulletScript : MonoBehaviourPunCallbacks
{
    private static float echoSpawnTimeConstraint;
    [SerializeField] private float bulletSpeed = 15;
    public bool isCollided;

    [HideInInspector] public Vector2 startDir;
    [SerializeField] private GameObject bulletTrail;
    private Rigidbody2D bulletRigidBody;
    private IEnumerator coroutine;
    private Vector2 direction;
    private Collider2D playerCollider;

    private void Start()
    {
        gameObject.GetComponent<AudioSource>().pitch = Random.Range(1f, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            LeanPool.Despawn(gameObject);
            StopCoroutine(coroutine);
        }

        if (collision.gameObject.name == "xAxisWall" || collision.gameObject.name == "yAxisWall")
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), playerCollider,
                false);
            collision.gameObject.GetComponent<AudioSource>().pitch = Random.Range(1f, 3f);
            collision.gameObject.GetComponent<AudioSource>().Play();
            direction = collision.GetContact(0).normal;
            if (collision.gameObject.name == "xAxisWall")
            {
                if (bulletRigidBody.velocity.normalized.x > 0)
                    direction.x += 2;
                else
                    direction.x += -2;
            }
            else
            {
                if (bulletRigidBody.velocity.normalized.y > 0)
                    direction.y += 2;
                else
                    direction.y += -2;
            }


            bulletRigidBody.velocity = direction.normalized * bulletSpeed;

            if (echoSpawnTimeConstraint == 0)
            {
                TrailGeneration.instance.TrailManagerStartEcho(collision, bulletRigidBody.velocity.normalized,
                    collision.GetContact(0).point);
                StartCoroutine(WaitToSpawnEchoAgain());
            }
        }
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.tag == "Player")
    //     {
    //         LeanPool.Despawn(gameObject);
    //         StopCoroutine(coroutine);
    //     }
    // }

    public void MoveBullet(Vector3 direction, Collider2D playerCol)
    {
        playerCollider = playerCol;
        bulletRigidBody = gameObject.GetComponent<Rigidbody2D>();
        bulletRigidBody.velocity = direction.normalized * bulletSpeed;
        coroutine = FadeBulletCoroutine();
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitToSpawnEchoAgain()
    {
        var duration = 2f;
        while (echoSpawnTimeConstraint < duration)
        {
            echoSpawnTimeConstraint += Time.deltaTime;
            yield return null;
        }

        echoSpawnTimeConstraint = 0;
    }

    private IEnumerator FadeBulletCoroutine()
    {
        var duration = 3f;
        float elapsedTime = 0;
        isCollided = true;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isCollided = false;
        echoSpawnTimeConstraint = 0;
        LeanPool.Despawn(gameObject);
    }
}