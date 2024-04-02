using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(turnOff());
    }

    private IEnumerator turnOff()
    {
        yield return new WaitForSeconds(4.0f);
        gameObject.SetActive(false);
    }
}
