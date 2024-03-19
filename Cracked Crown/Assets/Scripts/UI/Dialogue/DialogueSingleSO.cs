using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSingleOption", menuName = "Dialogue Single")]
public class DialogueSingleSO : ScriptableObject
{
    [SerializeField] public string dialogue;
    [SerializeField] public UIManager.characterIcons icon;
}
