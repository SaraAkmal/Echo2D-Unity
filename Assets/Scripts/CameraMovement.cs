using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothTime = 1F;
    private Vector3 velocity = Vector3.zero;
    private Boundaries cameraBoundary;

    private void Start()
    {
        cameraBoundary = GetComponent<Boundaries>();
    }

    private void Update()
    {
        Vector3 newPosition = cameraBoundary.IsObjectOutOfLimits(20, 40, target);
        if (newPosition.sqrMagnitude != 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(newPosition.x, newPosition.y, -2),
            ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.transform.position.x, target.transform.position.y, -2),
                ref velocity, smoothTime);
        }
    }
}