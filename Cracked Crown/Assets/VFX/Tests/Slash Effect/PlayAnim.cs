using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayAnim : MonoBehaviour
{
    // Start is called before the first frame update

    public VisualEffect sword;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            sword.Play();
        }
    }
}
