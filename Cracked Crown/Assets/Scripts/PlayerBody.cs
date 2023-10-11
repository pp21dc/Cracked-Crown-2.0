using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private Rigidbody rb;

    private void FixedUpdate()
    {
        float zInput = controller.ForwardMagnitude;
        float xInput = controller.HorizontalMagnitude;

        Vector3 movementVector = new Vector3(xInput, 0, zInput); // swapped z and x so that movement feels like its rotated 45 degrees for isometric
        if (movementVector.magnitude > 1)
        {
            movementVector.Normalize();
        }
        movementVector = rb.position + (movementVector * movementSpeed * Time.deltaTime);

        rb.MovePosition(movementVector);
    }
}
