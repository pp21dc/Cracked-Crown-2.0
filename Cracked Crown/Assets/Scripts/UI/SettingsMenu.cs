using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private bool isFullscreen = true;

    [SerializeField]
    Slider MasterSlider;
    [SerializeField]
    Slider SFXSlider;
    [SerializeField]
    Slider MusicSlider;

    [SerializeField]
    TMP_Dropdown Resolution;

    [SerializeField]
    private AudioMixer Mixer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setResolution()
    {
        if (Resolution.value == 0)
        {
            Screen.SetResolution(1920, 1080, isFullscreen);
        }
        else if (Resolution.value == 1)
        {
            Screen.SetResolution(2560, 1440, isFullscreen);
        }
        else if (Resolution.value == 2)
        {
            Screen.SetResolution(1280, 720, isFullscreen);
        }
    }
    
    public void setMasterVol()
    {
        Mixer.SetFloat("MasterVol", MasterSlider.value);
    }

    public void setSFXVol()
    {
        Mixer.SetFloat("SFXVol", SFXSlider.value);
    }

    public void setMusicVol()
    {
        Mixer.SetFloat("MusicVol", MusicSlider.value);
    }
}
