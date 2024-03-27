using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class ResStoneLights : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gm;

    [SerializeField] Light2D[] lights;

    Vector3 pos;
    void Awake()
    {
        gm = GameManager.Instance;
        pos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < gm.Players.Length; i++)
        {
            float distance = Vector3.Distance(pos, gm.Players[i].PB.transform.position);
            //Debug.Log(distance);
            lights[i].intensity = ((-Mathf.Log(distance)*10+3) * 40) + 1700;
            if(lights[i].intensity < 0)
            {
                lights[i].intensity = 0;
            }
        }
    }
}
