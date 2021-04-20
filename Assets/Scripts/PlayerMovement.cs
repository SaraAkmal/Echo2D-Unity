using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private JoystickControl joystick;
    [SerializeField] private float playerSpeed;
    private Boundaries playerBoundary;
    private Rigidbody2D playerRigidBody;
    public Vector2 playerDirection { get; private set; } //direction

    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerBoundary = GetComponent<Boundaries>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (joystick.joystickVec.y != 0)
        {
            playerRigidBody.velocity = new Vector2(joystick.joystickVec.x * playerSpeed, joystick.joystickVec.y * playerSpeed);
            playerDirection = playerRigidBody.velocity.normalized;
            Vector3 newPosition = playerBoundary.IsObjectOutOfLimits(25, 45, this.gameObject);
            if (newPosition.sqrMagnitude != 0)
            {
                transform.position = new Vector3(newPosition.x, newPosition.y, 0);
            }
        }
        else
        {
            playerRigidBody.velocity = Vector2.zero;
        }
    }
}