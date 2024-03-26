using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNode : MonoBehaviour
{
    
    [SerializeField] Vector2[] xy;
    [SerializeField] Vector2[] xySpeeds;

    [SerializeField] char[] xyPos;
    [SerializeField] Material mat;

    private void Start()
    {
        //Starting size
        mat.SetFloat("_blob1Size", Random.Range(0.05f, 0.08f));
        mat.SetFloat("_blob2Size", Random.Range(0.05f, 0.08f));
        mat.SetFloat("_blob3Size", Random.Range(0.05f, 0.08f));
        mat.SetFloat("_blob4Size", Random.Range(0.05f, 0.08f));
        mat.SetFloat("_blob5Size", Random.Range(0.05f, 0.08f));
        mat.SetFloat("_blob6Size", Random.Range(0.05f, 0.08f));

        
        for (int i = 0; i < 6; i++)
        {
            //startingPos
            xy[i].x = Random.Range(-0.5f, 0.5f);
            xy[i].y = Random.Range(-0.1f, 0.1f);

            //speeds
            xySpeeds[i].x = Random.Range(-0.08f, 0.08f);
            xySpeeds[i].y = Random.Range(-0.03f, 0.03f);
        }
    }

    private void Update()
    {
        for(int i = 0; i < xy.Length; i++)
        {
            xy[i] += new Vector2(Time.deltaTime * xySpeeds[i].x, Time.deltaTime * xySpeeds[i].y);

            if (xy[i].x < -0.5f)
            {
                xySpeeds[i].x *= -1;
                xy[i].x = -0.5f;
            }
            if (xy[i].x > 0.5f)
            {
                xySpeeds[i].x *= -1;
                xy[i].x = 0.5f;
            }
                
            if (xy[i].y < -0.1f)
            {
                xySpeeds[i].y *= -1;
                xy[i].y = -0.1f;
            }
            if (xy[i].y > 0.1f)
            {
                xySpeeds[i].y *= -1;
                xy[i].y = 0.1f;
            }
            if(i == 0)
            Debug.LogWarning(i + ": " + xySpeeds[i]);
            
            char[] tempArray = i.ToString().ToCharArray();
            Debug.LogWarning(tempArray[0]);
            xyPos[6] = tempArray[0];
            string s = new string(xyPos);
            mat.SetVector(s, xy[i]);
        }
        
    }
}
