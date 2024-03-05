using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shockWave : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine("Death");
    }


    IEnumerator Death ()
    {
        yield return new WaitForSeconds(1.25f);
        Destroy(gameObject);
    }
}
