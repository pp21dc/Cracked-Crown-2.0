using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlayerLayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BushCollision") || other.gameObject.CompareTag("Medium") || other.gameObject.CompareTag("Heavy") || other.gameObject.CompareTag("Light"))
            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Behind Bush");

        Debug.Log("Enter Trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BushCollision") || other.gameObject.CompareTag("Medium") || other.gameObject.CompareTag("Heavy") || other.gameObject.CompareTag("Light"))
            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Infront of Bush");

        Debug.Log("Exit Trigger");
    }
}
