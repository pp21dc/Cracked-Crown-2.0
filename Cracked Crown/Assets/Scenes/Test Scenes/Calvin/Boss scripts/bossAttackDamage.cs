using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takeDamage : MonoBehaviour
{
    [SerializeField]
    private GameObject Claw;
    private PlayerBody Playerbody;
    [SerializeField]
    private float damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Claw.GetComponent<BossPhases>().isClawSmash == true)
            {
                Playerbody = other.gameObject.GetComponent<PlayerBody>();
                Playerbody.DecHealth(damage);
            }
        }
    }
}
