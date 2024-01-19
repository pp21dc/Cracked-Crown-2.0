using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{

    [SerializeField]
    Animator Animator;
    [SerializeField]
    GameObject CharacterFolder;

    private bool moving;
    [HideInInspector]
    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }

    public bool dashing;

    private bool dead;
    [HideInInspector]
    public bool Dead
    {
        get { return dead; }
        set { dead = value; }
    }

    private bool attacking;
    [HideInInspector]
    public bool Attacking
    {
        get { return attacking; }
        set { attacking = value; }
    }

    PlayerContainer PC;

    private void Awake()
    {
        PC = transform.parent.GetComponent<PlayerContainer>();
        PC.PAC = this;
    }

    private void FixedUpdate()
    {
        if (Animator != null)
        {
            Animator.SetBool("Moving", moving);
            Animator.SetBool("Dashing", dashing);
            Animator.SetBool("Attacking", attacking);
        }
        else if (CharacterFolder.transform.GetChild(0).GetChild(0).TryGetComponent<Animator>(out Animator anim))
        {
            Animator = anim;
        }
        //PC = transform.parent.parent.parent.parent.GetComponent<PlayerContainer>();

    }

 

}
