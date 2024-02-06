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
        Debug.Log("Workfnhfguid");
        yield return new WaitForSeconds(1.25f);
        Destroy(gameObject);
    }
}
