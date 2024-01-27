using System.Collections;
using UnityEngine;

public class TrailMovement : MonoBehaviour
{
    public delegate void TrailManagerDespawnObject(GameObject spawnedObject);

    public bool isCollided;
    [SerializeField] private float trailSpeed = 15;
    private IEnumerator bouncingCoroutine;
    private Vector2 echObjectDirection;
    private Rigidbody2D echoRigidBody;
    private float inclineValue;

    private TrailManagerDespawnObject trailManagerDespawnObject;

    private void OnCollisionEnter2D(Collision2D collision) // it is called when object is spawned
    {
        if (collision.gameObject.name == "xAxisWall" || collision.gameObject.name == "yAxisWall")
        {
            if (isCollided) StopEcho();
            bouncingCoroutine = BounceEcho(collision, echObjectDirection);
            StartCoroutine(bouncingCoroutine);
        }
    }

    public void StartEcho(Collision2D collision, Vector3 direction,
        TrailManagerDespawnObject trailManagerDespawnFunction, float inclineValue)
    {
        trailManagerDespawnObject = trailManagerDespawnFunction;
        this.inclineValue = inclineValue;
        isCollided = true;
        bouncingCoroutine = BounceEcho(collision, direction); // collision information, player direction for reflection
        StartCoroutine(FadeCoroutine());
    }

    public void StartJumpEffect(Vector2 direction,
        TrailManagerDespawnObject trailManagerDespawnFunction, float inclineValue)
    {
        trailManagerDespawnObject = trailManagerDespawnFunction;
        StartCoroutine(StartJumpEchoIEnumerator(direction, inclineValue));
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator StartJumpEchoIEnumerator(Vector2 direction, float inclineValue)
    {
        var duration = 5f;
        float elapsedTime = 0;
        var magnitude = 0.5f;
        direction.x = inclineValue;

        echoRigidBody = gameObject.GetComponent<Rigidbody2D>();
        echoRigidBody.velocity = direction.normalized * trailSpeed * magnitude;


        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator BounceEcho(Collision2D collision, Vector2 direction)
    {
        var duration = 5f;
        float elapsedTime = 0;
        var magnitude = 0.5f;
        //direction = Vector3.Reflect(direction, collision.GetContact(0).normal) * magnitude;
        direction = collision.GetContact(0).normal * magnitude;
        if (collision.gameObject.name == "xAxisWall")
            direction.x = inclineValue;
        else
            direction.y = inclineValue;

        echoRigidBody = gameObject.GetComponent<Rigidbody2D>();
        echoRigidBody.velocity = direction.normalized * trailSpeed;


        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isCollided = false;
    }

    private void StopEcho()
    {
        StopCoroutine(bouncingCoroutine);
    }

    private IEnumerator FadeCoroutine()
    {
        var duration = 6f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        trailManagerDespawnObject(gameObject);
    }
}