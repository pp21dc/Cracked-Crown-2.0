using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource AS;

    [SerializeField]
    [Tooltip("This list can be exchanged depending on character type | Refer to 'enum' in script for order of audio placement in list")]
    private AudioClip[] Player_AudioClips;

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
