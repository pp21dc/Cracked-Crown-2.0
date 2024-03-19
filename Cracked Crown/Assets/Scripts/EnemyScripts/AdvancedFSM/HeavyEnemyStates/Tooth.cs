using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooth : MonoBehaviour
{
    [SerializeField]
    private float speed = 35f;

    private Vector3 Direction;

    [SerializeField]
    private Collider Disapear;


    private void Update()
    {
        transform.Translate(Direction.x + (speed * Time.deltaTime), Direction.y, Direction.z);
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
