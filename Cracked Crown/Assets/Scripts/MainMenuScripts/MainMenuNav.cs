using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuNav : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject Settings;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Submit"))
        {
            // clears current menu selection
            EventSystem.current.SetSelectedGameObject(null);
            // sets a new selected object
            EventSystem.current.SetSelectedGameObject(Settings);
        }
    }

}
