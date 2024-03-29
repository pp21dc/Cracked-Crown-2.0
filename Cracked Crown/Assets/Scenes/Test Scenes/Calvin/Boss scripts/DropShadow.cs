using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    [SerializeField]
    private GameObject followedObject;
    [SerializeField]
    private GameObject clawObject;
    [SerializeField]
    private BossPhases script;
    private float cush;
    private float folX;
    private float folY;
    private float folZ;
    void Start()
    {
        folY = followedObject.transform.localPosition.y;
        gameObject.transform.position = followedObject.transform.position - new Vector3 (0, 50f, 0); 
    }

    // Update is called once per frame
    void Update()
    {
        folX = followedObject.transform.position.x;
        folZ = followedObject.transform.position.z;
        if (script.displayAttack() == "PincerAttack")
        {
            if (script.clawreturn)
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(folX - 2, folY - 10, folZ), 0.5f);
            }
            else if (script.attacktimer >= 7.8f){
                cush = clawObject.transform.localPosition.y * 2;
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(folX - 2, folY - 14 - cush, folZ), 0.3f);
            }
            else
            {
                cush = clawObject.transform.localPosition.y * 2;
                gameObject.transform.position = new Vector3(folX - 2, folY - 14 - cush, folZ);
            }
        }
        else if (script.displayAttack() == "ClawSmash")
        {
            if (script.attacktimer > 4)
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(folX - 2, folY, folZ), 0.1f);
            }
            else if (script.attacktimer < 1.16f)
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(folX - 2, folY - 10, folZ), 0.3f);
            }
            else
            {
                gameObject.transform.position = new Vector3(folX - 2, folY, folZ);
            }
        }
        else
        {
            gameObject.transform.position = new Vector3(folX - 2, folY - 10, folZ);
        }
    }
}
