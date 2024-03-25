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

    private bool finishing_medium;
    public bool Finishing_Medium
    {
        get { return finishing_medium; }
        set { finishing_medium = value; }
    }

    private bool finishing_medium_green;
    public bool Finishing_Medium_Green
    {
        get { return finishing_medium; }
        set { finishing_medium = value; }
    }

    private bool finishing_medium_purple;
    public bool Finishing_Medium_Purple
    {
        get { return finishing_medium; }
        set { finishing_medium = value; }
    }

    private bool finishing_medium_red;
    public bool Finishing_Medium_Red
    {
        get { return finishing_medium; }
        set { finishing_medium = value; }
    }

    private bool finishing_light;
    public bool Finishing_Light
    {
        get { return finishing_light; }
        set { finishing_light = value; }
    }

    private bool finishing_light_green;
    public bool Finishing_Light_Green
    {
        get { return finishing_light_green; }
        set { finishing_light_green = value; }
    }

    private bool finishing_light_purple;
    public bool Finishing_Light_Purple
    {
        get { return finishing_light_purple; }
        set { finishing_light_purple = value; }
    }

    private bool finishing_light_red;
    public bool Finishing_Light_Red
    {
        get { return finishing_light_red; }
        set { finishing_light_red = value; }
    }

    private bool finishing_heavy;
    public bool Finishing_Heavy
    {
        get { return finishing_heavy; }
        set { finishing_heavy = value; }
    }

    private bool finishing_heavy_green;
    public bool Finishing_Heavy_Green
    {
        get { return finishing_heavy_green; }
        set { finishing_heavy_green = value; }
    }

    private bool finishing_heavy_purple;
    public bool Finishing_Heavy_Purple
    {
        get { return finishing_heavy_purple; }
        set { finishing_heavy_purple = value; }
    }

    private bool finishing_heavy_red;
    public bool Finishing_Heavy_Red
    {
        get { return finishing_heavy_red; }
        set { finishing_heavy_red = value; }
    }

    PlayerContainer PC;

    private void Awake()
    {
        PC = transform.parent.GetComponent<PlayerContainer>();
        PC.PAC = this;
    }

    public void Finisher(string size, string colour, bool active)
    {
        if (size == "Light")
        {
            if (colour == "Green")
                finishing_light_green = active;
            if (colour == "Purple")
                finishing_light_purple = active;
            if (colour == "Red")
                finishing_light_red = active;


        }
        else if (size == "Medium")
        {
            if (colour == "Green")
                finishing_medium_green = active;
            if (colour == "Purple")
                finishing_medium_purple = active;
            if (colour == "Red")
                finishing_medium_red = active;
        }
        else if (size == "Heavy")
        {
            if (colour == "Green")
                finishing_heavy_green = active;
            if (colour == "Purple")
                finishing_heavy_purple = active;
            if (colour == "Red")
                finishing_heavy_red = active;
        }
    }

    public void SetFinishers(bool active)
    {
        finishing_medium_green = active;
        finishing_medium_purple = active;
        finishing_medium_red = active;
        finishing_light_green = active;
        finishing_light_purple = active;
        finishing_light_red = active;
        finishing_heavy_green = active;
        finishing_heavy_purple = active;
        finishing_heavy_red = active;
    }

    private void FixedUpdate()
    {
        if (Animator != null && PC.PB.CharacterType != null)
        {
            Animator.runtimeAnimatorController = PC.PB.CharacterType.controller;
            Animator.SetBool("Moving", moving);
            Animator.SetBool("Dashing", dashing);
            Animator.SetBool("Attacking", attacking);
            Animator.SetBool("FinishMed_Green", finishing_medium_green);
            Animator.SetBool("FinishMed_Purple", finishing_medium_purple);
            Animator.SetBool("FinishMed_Red", finishing_medium_red);
            Animator.SetBool("FinishLight_Green", finishing_light_green);
            Animator.SetBool("FinishLight_Purple", finishing_light_purple);
            Animator.SetBool("FinishLight_Red", finishing_light_red);
            Animator.SetBool("FinishHeavy_Green", finishing_heavy_green);
            Animator.SetBool("FinishHeavy_Purple", finishing_heavy_purple);
            Animator.SetBool("FinishHeavy_Red", finishing_heavy_red);
            Animator.SetFloat("Input_Horz", PC.PB.controller.HorizontalMagnitude);
            Animator.SetFloat("Input_Vert", PC.PB.controller.ForwardMagnitude);
            //if (finishing) { finishing = false; }
            if (true )
            {
                Animator.SetBool("Dead", dead);
            }
        }
        else if (CharacterFolder.transform.GetChild(0).GetChild(0).TryGetComponent<Animator>(out Animator anim))
        {
            Animator = anim;
        }
        
        //PC = transform.parent.parent.parent.parent.GetComponent<PlayerContainer>();

    }

 

}
