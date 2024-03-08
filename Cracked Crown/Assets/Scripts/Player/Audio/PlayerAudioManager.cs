using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource AS;
    [SerializeField]
    PlayerAnimEventHandler PAEH;
    [SerializeField]
    PlayerContainer PC;
    [SerializeField]
    [Tooltip("This list can be exchanged depending on character type | Refer to 'enum' in script for order of audio placement in list")]
    private AudioClip[] Player_AudioClips;

    private void Start()
    {
        PC = PAEH.PC;
    }

    private void Update()
    {
        PC = PAEH.PC;
        if (PC != null )
            Player_AudioClips = PC.PB.CharacterType.soundeffects;
    }

    public enum AudioType
    {
        EmptySwing,
        EnemyHit, 
        PlayerHit,
        Swing,
        Death,
        Move,
        Dash
    }

    public AudioType[] audioTypes;

    public void NewClip(AudioClip newClipToPlay)
    {
        AS.Stop();
        AS.clip = newClipToPlay;
        AS.Play();
    }

    public void PlayAudio(AudioType type)
    {
        NewClip(Player_AudioClips[(int)type]);
    }


}
