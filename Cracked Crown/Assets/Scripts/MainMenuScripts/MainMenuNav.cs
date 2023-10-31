using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuNav : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject Settings;

    GameObject CurrentSelection;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Submit"))
        {
            // finds currently selected GameObject
            CurrentSelection = EventSystem.current.currentSelectedGameObject;
            // Acts based on the GameObject selected
            CurrentSelection.SetActive(false);
        }
    }

}
