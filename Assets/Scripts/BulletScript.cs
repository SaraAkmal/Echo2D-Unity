using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class BulletScript : MonoBehaviour
{
    private static float echoSpawnTimeConstraint = 0;
    private Rigidbody2D bulletRigidBody;
    [SerializeField] private float bulletSpeed = 15;
    private IEnumerator coroutine;
    public bool isCollided = false;

    [HideInInspector] public Vector2 startDir;

    private IEnumerator MoveBulletCoroutine()
    {
        float duration = 2f;
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

    public void MoveBullet(Vector3 direction)
    {
        bulletRigidBody = gameObject.GetComponent<Rigidbody2D>();
        bulletRigidBody.velocity = direction.normalized * bulletSpeed;
        coroutine = MoveBulletCoroutine();
        StartCoroutine(coroutine);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "xAxisWall" || collision.gameObject.name == "yAxisWall")
        {
            Vector2 direction = collision.GetContact(0).normal;
            if (collision.gameObject.name == "xAxisWall")
            {
                direction.x += Random.Range(-2, 2);
            }
            else
            {
                direction.y += Random.Range(-2, 2);
            }

            bulletRigidBody.velocity = direction.normalized * bulletSpeed;
            if (isCollided)
            {
                StopCoroutine(coroutine);
                coroutine = BounceBullet();
                StartCoroutine(coroutine);
            }

            if (echoSpawnTimeConstraint == 0)
            {
                TrailGeneration.instance.TrailManagerStartEcho(collision, bulletRigidBody.velocity.normalized, collision.GetContact(0).point);
                StartCoroutine(waitToSpawnEchoAgain());
            }
        }
        if (collision.gameObject.name == "Player")
        {
            //generate particles effect
        }
    }

    private IEnumerator waitToSpawnEchoAgain()
    {
        float duration = 4f;
        while (echoSpawnTimeConstraint < duration)
        {
            echoSpawnTimeConstraint += Time.deltaTime;
            yield return null;
        }
        echoSpawnTimeConstraint = 0;
    }

    private IEnumerator BounceBullet()
    {
        float duration = 5f;
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