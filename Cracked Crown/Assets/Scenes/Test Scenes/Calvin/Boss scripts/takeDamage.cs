using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takeDamage : MonoBehaviour
{
    [SerializeField]
    private BossPhases bossScript;
    private float attackvalue;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAttack")
        {
            attackvalue = other.gameObject.transform.GetChild(1).GetComponent<PlayerBody>().damage;
            bossScript.decHealth(attackvalue);
        }
    }
}
