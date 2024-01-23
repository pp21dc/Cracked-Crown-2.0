using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    Material player1Health;

    PlayerBody player1Body;

    
    // Start is called before the first frame update
    void Start()
    {
        player1Body = GameObject.Find("Player Body").GetComponent<PlayerBody>();
    }

    // Update is called once per frame
    void Update()
    {

        player1Health.SetFloat("_Position", 1 - (player1Body.Health/ 10));
    }
}
