using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{

    [SerializeField]
    AnimatorController AC;

    private bool moving;
    [HideInInspector]
    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }

    private bool dashing;
    [HideInInspector]
    public bool Dashing
    {
        get { return Dashing; }
        set { Dashing = value; }
    }

    private bool dead;
    [HideInInspector]
    public bool Dead
    {
        get { return dead; }
        set { dead = value; }
    }

    private void Update()
    {
        AC.parameters.SetValue(dead, 0);
        AC.parameters.SetValue(moving, 1);
        AC.parameters.SetValue(dashing, 2);
    }

 

}
