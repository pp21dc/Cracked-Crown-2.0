using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private GameObject[] Players;

    public GameObject[] players { get { return Players; } }

    [SerializeField]
    private GameObject group;

    

    private void Awake()
    {

        

    }
}
