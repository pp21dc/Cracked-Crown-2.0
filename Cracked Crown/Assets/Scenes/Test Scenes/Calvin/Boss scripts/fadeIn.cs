using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    float alpha;
    SpriteRenderer sr;
    [SerializeField]
    float fadetime;
    private void Awake()
    {
        alpha = 0f;
        sr = gameObject.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (alpha < 1)
        {
            Debug.Log(alpha);
            StartCoroutine(fadeIn(alpha));
            alpha += Time.deltaTime/fadetime;
        }
        else
        {
            alpha = 1;
            enabled = false;
        }
    }
    public IEnumerator fadeIn (float a)
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);

        yield return new WaitForEndOfFrame();
    }
}
