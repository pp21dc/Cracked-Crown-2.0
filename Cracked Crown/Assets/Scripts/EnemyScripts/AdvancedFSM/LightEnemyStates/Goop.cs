using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goop : MonoBehaviour
{
    [SerializeField]
    private float speed = 12f;

    private Vector3 Direction;

    [SerializeField]
    private Collider Disapear;


    private void Update()
    {
        transform.Translate(Direction * speed * Time.deltaTime);
    }

    private void OnEnable()
    {
        StartCoroutine(SelfDestruct());
    }

    public void Fire(Vector3 direction)
    {
        Direction = direction;
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBody player = other.GetComponent<PlayerBody>();
            player.DecHealth(3f);
            gameObject.SetActive(false);
        }
    }

}
