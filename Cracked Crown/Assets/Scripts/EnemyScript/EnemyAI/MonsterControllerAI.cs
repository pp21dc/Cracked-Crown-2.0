using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[System.Serializable]
public abstract class AIProperties
{
    public float speed = 3.0f;
    public float rotSpeed = 2.0f;
    public float chaseDistance = 20;
    public float healthRegenRate = 0;
}

public class MonsterControllerAI : AdvancedFSM
{
    [SerializeField]
    private bool debugDraw;
    [SerializeField]
    private Text StateText;
    [SerializeField]
    private Text HealthText;

    [SerializeField]
    private GameObject deathGO;
    public GameObject DeathGO
    {
        get { return deathGO; }
    }



    private float health;
    public float Health
    {
        get { return health; }
    }
    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); }
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }

    private string GetStateString()
    {

        string state = "NONE";
        if (CurrentState.ID == FSMStateID.Dead)
        {
            state = "DEAD";
        }

        return state;
    }

    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        health = 100;
        ConstructFSM();
    }

    protected override void FSMUpdate()
    {

        if (CurrentState != null)
        {
            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
        }
        StateText.text = "MONSTER STATE IS: " + GetStateString();
        HealthText.text = "MONSTER HEALTH IS: " + Health;

        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.red);
        }
    }


    private void ConstructFSM()
    {
        //
        //Create States
        //

        //Create the Dead state
       // DeadState deadState = new DeadState(this);
        //there are no transitions out of the dead state
       // AddFSMState(deadState);
    }

    public void StartDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        Renderer r = GetComponent<Renderer>();
        r.enabled = false;

        deathGO.SetActive(true);

        yield return new WaitForSeconds(2.2f);

        Destroy(gameObject);
    }


}