using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypePrimaryAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyObject());
    }

    IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}
