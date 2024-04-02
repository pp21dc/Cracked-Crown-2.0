using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EyeMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float forces = 25;
    GameManager manager;

    [SerializeField]
    GameObject Mesh;
    [SerializeField]
    GameObject DropShadow;

    private void Start()
    {
        manager = GameManager.Instance;
        rb.AddForce(new Vector3((Random.Range(-5, 5) + 1) * forces, forces * 5, (Random.Range(-5, 5) + 1) * forces), ForceMode.Impulse);
    }

    private void Update()
    {
        Mesh.transform.LookAt(manager.cam);
        Vector3 pos = transform.position;
        pos.y = 0;
        DropShadow.transform.position = pos;
    }

    private void FixedUpdate()
    {
        rb.AddForce(0, -9.8f*250, 0, ForceMode.Force);
    }

}
