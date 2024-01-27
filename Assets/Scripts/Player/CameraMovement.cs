using Photon.Pun;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float smoothTime = 1F;
    [SerializeField] private float zOffset;
    private GameObject camera;
    private Boundaries cameraBoundary;
    private PhotonView photonView;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        cameraBoundary = GetComponent<Boundaries>();
        camera = GameObject.FindWithTag("MainCamera");
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (photonView.IsMine)
            // var newPosition = cameraBoundary.IsObjectOutOfLimits(20, 40, gameObject);
            // if (newPosition.sqrMagnitude != 0)
            //     camera.transform.position = Vector3.SmoothDamp(camera.transform.position,
            //         new Vector3(newPosition.x, newPosition.y, zOffset),
            //         ref velocity, smoothTime);
            // else
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position,
                new Vector3(transform.position.x, transform.position.y, zOffset),
                ref velocity, smoothTime);
    }
}