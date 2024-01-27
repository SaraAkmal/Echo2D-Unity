using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class TrailGeneration : MonoBehaviour
{
    public static TrailGeneration instance;
    [SerializeField] private GameObject echoObject;
    [SerializeField] private int numOfEchos; // must be odd
    [SerializeField] private float distanceBetweenEchos; // must be odd
    private List<GameObject> echoObjList;
    private GameObject spawnedObject;

    private void Start()
    {
        instance = this;
        echoObjList = new List<GameObject>();
        PreloadEchoObjects();
    }
    

    private void PreloadEchoObjects()
    {
        for (var i = 0; i < 10; i++) echoObjList.Add(LeanPool.Spawn(echoObject));
        for (var i = 0; i < 10; i++)
            LeanPool.Despawn(echoObjList[i]);
    }

    public void TrailManagerStartEcho(Collision2D collision, Vector2 direction, Vector3 position)
    {
        SpawnEchoObject(collision, direction, 0).transform.position = position;
        for (var i = 1; i <= (numOfEchos - 1) / 2; i++)
            SpawnEchoObject(collision, direction, i * distanceBetweenEchos).transform.position = position;
        for (var i = 1; i <= (numOfEchos - 1) / 2; i++)
            SpawnEchoObject(collision, direction, -i * distanceBetweenEchos).transform.position = position;
    }

    public void TrailManagerJumpEcho(Vector2 position)
    {
        SpawnJumpEchoObject(new Vector2(-0.5f, -0.5f), 0).transform.position = position;
        for (var i = 1; i <= 9 / 2; i++)
            SpawnJumpEchoObject(new Vector2(-0.5f, -0.5f), i * 0.5f).transform.position = position;
        for (var i = 1; i <= 9 / 2; i++)
            SpawnJumpEchoObject(new Vector2(-0.5f, -0.5f), -i * 0.5f).transform.position = position;

        SpawnJumpEchoObject(new Vector2(0.5f, 0.5f), 0).transform.position = position;
        for (var i = 1; i <= 9 / 2; i++)
            SpawnJumpEchoObject(new Vector2(0.5f, 0.5f), i * 0.5f).transform.position = position;
        for (var i = 1; i <= 9 / 2; i++)
            SpawnJumpEchoObject(new Vector2(0.5f, 0.5f), -i * 0.5f).transform.position = position;
    }


    public void DespawnEchoObject(GameObject spawnedObject)
    {
        LeanPool.Despawn(spawnedObject);
    }

    public GameObject SpawnEchoObject(Collision2D collision, Vector2 direction,float inclineValue)
    {
        spawnedObject = LeanPool.Spawn(echoObject);
        spawnedObject.GetComponent<TrailMovement>().StartEcho(collision, direction, DespawnEchoObject, inclineValue);
        return spawnedObject;
    }

    public GameObject SpawnJumpEchoObject(Vector2 direction, float inclineValue)
    {
        spawnedObject = LeanPool.Spawn(echoObject);
        spawnedObject.GetComponent<TrailMovement>().StartJumpEffect(direction, DespawnEchoObject, inclineValue);
        return spawnedObject;
    }
}