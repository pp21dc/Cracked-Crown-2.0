using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShockwave : MonoBehaviour
{
    [SerializeField]
    private float SHOCKWAVESPEED;
    [SerializeField]
    private float MAXRANGE;
    private Vector3 STOPPOINT;
    // Start is called before the first frame update
    void Awake()
    {
        STOPPOINT = new Vector3 (MAXRANGE, 0, MAXRANGE);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.localScale.x < STOPPOINT.x)
        {
            gameObject.transform.localScale += new Vector3(SHOCKWAVESPEED * Time.deltaTime, 0, SHOCKWAVESPEED * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("hit");
            // player takes damage and movement is frozen
        }
    }
}
