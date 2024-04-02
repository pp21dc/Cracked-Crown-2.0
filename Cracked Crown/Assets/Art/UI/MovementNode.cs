using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNode : MonoBehaviour
{
    
    [SerializeField] Vector2[] xy;
    [SerializeField] Vector2[] xySpeeds;

    [SerializeField] char[] xyPos;
    [SerializeField] Material mat;

    [SerializeField] float leftLimit = 0.5f;
    [SerializeField] float rightLimit = -0.5f;
    [SerializeField] float topLimit = -0.1f;
    [SerializeField] float bottomLimit = 0.1f;

    [SerializeField] float maxSpeedX = 0.08f;
    [SerializeField] float maxSpeedY = 0.03f;

    [SerializeField] float blobMaxSize = 0.08f;
    [SerializeField] float blobMinSize = 0.05f;
    private void Start()
    {
        //Starting size
        mat.SetFloat("_blob1Size", Random.Range(blobMinSize, blobMaxSize));
        mat.SetFloat("_blob2Size", Random.Range(blobMinSize, blobMaxSize));
        mat.SetFloat("_blob3Size", Random.Range(blobMinSize, blobMaxSize));
        mat.SetFloat("_blob4Size", Random.Range(blobMinSize, blobMaxSize));
        mat.SetFloat("_blob5Size", Random.Range(blobMinSize, blobMaxSize));
        mat.SetFloat("_blob6Size", Random.Range(blobMinSize, blobMaxSize));

        
        for (int i = 0; i < 6; i++)
        {
            //startingPos
            xy[i].x = Random.Range(rightLimit, leftLimit);
            xy[i].y = Random.Range(topLimit, bottomLimit);

            //speeds
            xySpeeds[i].x = Random.Range(-maxSpeedX, maxSpeedX);
            xySpeeds[i].y = Random.Range(-maxSpeedY, maxSpeedY);
        }
    }

    private void Update()
    {
        for(int i = 0; i < xy.Length; i++)
        {
            xy[i] += new Vector2(Time.deltaTime * xySpeeds[i].x, Time.deltaTime * xySpeeds[i].y);

            if (xy[i].x < rightLimit)
            {
                xySpeeds[i].x *= -1;
                xy[i].x = rightLimit;
            }
            if (xy[i].x > leftLimit)
            {
                xySpeeds[i].x *= -1;
                xy[i].x = leftLimit;
            }
                
            if (xy[i].y < topLimit)
            {
                xySpeeds[i].y *= -1;
                xy[i].y = topLimit;
            }
            if (xy[i].y > bottomLimit)
            {
                xySpeeds[i].y *= -1;
                xy[i].y = bottomLimit;
            }
            //if(i == 0)
            //Debug.LogWarning(i + ": " + xySpeeds[i]);
            
            char[] tempArray = i.ToString().ToCharArray();
            //Debug.LogWarning(tempArray[0]);
            xyPos[6] = tempArray[0];
            string s = new string(xyPos);
            mat.SetVector(s, xy[i]);
        }
        
    }
}
