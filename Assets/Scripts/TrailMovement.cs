using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class TrailMovement : MonoBehaviour
{
    public bool isCollided = false;
    private IEnumerator coroutine;
    private Vector2 echObjectDirection;
    private float inclineValue;
    [SerializeField] private float trailSpeed = 15;

    public delegate void TrailManagerDespawnObject(GameObject spawnedObject);

    private TrailManagerDespawnObject trailManagerDespawnObject;

    public void StartEcho(Collision2D collision, Vector3 direction, TrailManagerDespawnObject trailManagerDespawnFunction, float inclineValue)
    {
        trailManagerDespawnObject = trailManagerDespawnFunction;
        this.inclineValue = inclineValue;
        isCollided = true;
        coroutine = BounceEcho(collision, direction, trailManagerDespawnObject);
        StartCoroutine(FadeCourtine()); // collision information, player direction for reflection
    }

    private IEnumerator BounceEcho(Collision2D collision, Vector3 direction, TrailManagerDespawnObject trailManagerDespawnObject)
    {
        float duration = 5f;
        float elapsedTime = 0;
        float magnitude = 0.5f;
        //direction = Vector3.Reflect(direction, collision.GetContact(0).normal) * magnitude;
        direction = collision.GetContact(0).normal * magnitude;

        print(collision.gameObject.name);
        if (collision.gameObject.name == "xAxisWall")
        {
            print("xAxisWall");
            direction.x = inclineValue;
        }
        else
        {
            print("yAxisWall");
            direction.y = inclineValue;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position += direction.normalized * trailSpeed * Time.deltaTime;
            echObjectDirection = transform.position.normalized;
            yield return null;
        }
        isCollided = false;
    }

    private void StopEcho()
    {
        StopCoroutine(coroutine);
    }

    private void OnCollisionEnter2D(Collision2D collision) // it is called when object is spawned
    {
        if (collision.gameObject.name == "xAxisWall" || collision.gameObject.name == "yAxisWall")
        {
            if (isCollided)
            {
                StopEcho();
            }
            coroutine = BounceEcho(collision, echObjectDirection, trailManagerDespawnObject);
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator FadeCourtine()
    {
        float duration = 5f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        trailManagerDespawnObject(this.gameObject);
    }
}