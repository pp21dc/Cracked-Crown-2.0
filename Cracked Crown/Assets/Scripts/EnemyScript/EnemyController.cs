using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private GameObject[] Players = new GameObject[4];

    public GameObject[] players { get { return Players; } }

    private GameObject playerLocator;

    

    private void Awake()
    {

        Players = GameObject.FindGameObjectsWithTag("AddPlayer");


    }
}
