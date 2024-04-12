using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseDelete : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "BeenRevived")
        {
            //StartCoroutine(destroyIncaseOfBug());
        }
    }

    private IEnumerator destroyIncaseOfBug()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
