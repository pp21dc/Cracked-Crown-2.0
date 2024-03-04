using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashRedTest : MonoBehaviour
{
    [SerializeField]
    float flashTime = 0.15f;

    [SerializeField]
    SpriteRenderer sp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(FlashRed(sp));
        }
    }

    public IEnumerator FlashRed(SpriteRenderer s)
    {
        s.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        s.color = Color.white;

    }
}
