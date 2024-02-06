using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finishable : MonoBehaviour
{

    float timer = 0f;
    bool on = false;

    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 1f)
        {
            if (on)
            {
                on = false;
                sprite.color = Color.red;
            }
            else
            {
                on = true;
                sprite.color = Color.white;
            }
        }
    }
}
