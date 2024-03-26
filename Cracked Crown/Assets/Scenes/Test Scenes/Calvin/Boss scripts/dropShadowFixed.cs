using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropShadowFixed : MonoBehaviour
{
    [SerializeField]
    private GameObject claw;
    [SerializeField]
    private BossPhases bossphases;
    [SerializeField]
    private float magnitude;
    private float folX;
    private float folY;
    private float folZ;
    // Update is called once per frame
    void Start()
    {
        folY = claw.transform.position.y;
    }

    void Update()
    {
        folX = claw.transform.position.x;
        folZ = claw.transform.position.z;
        gameObject.transform.position = new Vector3(folX + 4, folY - 70, folZ);

        if (bossphases.displayAttack() == "PincerAttack")
        {
            float displacement = (folY + claw.transform.position.y) / magnitude;
            gameObject.transform.position = new Vector3(folX + 4, folY - 70, folZ - 60 * displacement);
        }
    }
}
