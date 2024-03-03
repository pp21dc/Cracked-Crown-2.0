using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoIAnimation : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Image image;

    private void Update()
    {
        image.sprite = spriteRenderer.sprite;
    }
}
