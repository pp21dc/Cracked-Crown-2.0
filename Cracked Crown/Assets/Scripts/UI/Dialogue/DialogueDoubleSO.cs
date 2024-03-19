using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDoubleOption", menuName = "Dialogue Double")]
public class DialogueDoubleSO : ScriptableObject
{
    [SerializeField] string dialogue1;
    [SerializeField] int icon1;
    
    [SerializeField] string dialogue2;
    [SerializeField] int icon2;

}
