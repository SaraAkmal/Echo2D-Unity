using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class TrailGeneration : MonoBehaviour
{
    [SerializeField] private GameObject echoObject;
    [SerializeField] private int numOfEchos; // must be odd
    [SerializeField] private float distanceBetweenEchos; // must be odd
    private GameObject spawnedObject;
    private List<GameObject> echoObjList;

    public static TrailGeneration instance;

    private void Start()
    {
        instance = this;
        echoObjList = new List<GameObject>();
        PreloadEchoObjects();
    }

    private void PreloadEchoObjects()
    {
        for (int i = 0; i < 10; i++)
        {
            echoObjList.Add(LeanPool.Spawn(echoObject));
        }
        for (int i = 0; i < 10; i++)
            LeanPool.Despawn(echoObjList[i]);
    }

    public void TrailManagerStartEcho(Collision2D collision, Vector3 direction, Vector3 position)
    {
        SpawnEchoObject(collision, direction, 0).transform.position = position;
        for (int i = 1; i <= (numOfEchos - 1) / 2; i++)
        {
            SpawnEchoObject(collision, direction, i * distanceBetweenEchos).transform.position = position;
        }
        for (int i = 1; i <= (numOfEchos - 1) / 2; i++)
        {
            SpawnEchoObject(collision, direction, -i * distanceBetweenEchos).transform.position = position;
        }
    }

    public void DespawnEchoObject(GameObject spawnedObject)
    {
        LeanPool.Despawn(spawnedObject);
    }

    public GameObject SpawnEchoObject(Collision2D collision, Vector3 direction, float i)
    {
        spawnedObject = LeanPool.Spawn(echoObject);
        spawnedObject.GetComponent<TrailMovement>().StartEcho(collision, direction, DespawnEchoObject, i);
        return spawnedObject;
    }
}