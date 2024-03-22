using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public float speed;
    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range( FlockingManager.FM.minSpeed, FlockingManager.FM.maxSpeed );
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockingManager.FM.transform.position, FlockingManager.FM.swimLimits * 2);

        if(!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockingManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockingManager.FM.RotationSpeed * Time.deltaTime);
        }
        else
        {



            if (Random.Range(0, 100) < 5)
            {
                speed = Random.Range(FlockingManager.FM.minSpeed, FlockingManager.FM.maxSpeed);
            }

            if (Random.Range(0, 100) < 5)
            {
                ApplyRules();
            }

        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    public void ApplyRules()
    {
        GameObject[] gos;
        gos = FlockingManager.FM.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        //adding up all the flockers
        foreach ( GameObject go in gos ) 
        {
            if(go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                if (nDistance <= FlockingManager.FM.NeighboursDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f)
                    {
                       vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;

                }
            }
        }

        //averaging the flockers
        if(groupSize > 0)
        {
            vcenter = vcenter / groupSize + (FlockingManager.FM.currentGoalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > FlockingManager.FM.maxSpeed)
            {
                speed = FlockingManager.FM.maxSpeed;    
            }

            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero) 
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockingManager.FM.RotationSpeed * Time.deltaTime);
            }
        }

    }
}
