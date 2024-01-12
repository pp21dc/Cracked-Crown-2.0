using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private bool isFullscreen = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setRes1080()
    {
        Screen.SetResolution(1920, 1080, isFullscreen);
    }

    void setRes11440()
    {
        Screen.SetResolution(1440, 2560,isFullscreen);
    }

    void setRes720()
    {
        Screen.SetResolution(720, 1280, isFullscreen);
    }
}
