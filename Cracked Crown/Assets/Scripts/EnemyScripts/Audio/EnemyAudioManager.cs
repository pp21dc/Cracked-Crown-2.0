using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource AS;

    [SerializeField]
    [Tooltip("This list can be exchanged depending on character type")]
    private AudioClip[] Enemy_AudioClips;

    public enum AudioType
    {
        Attack,
        Die,
        Move,
        Shoot
    }

    public void NewClip(AudioClip newClipToPlay)
    {
        AS.Stop();
        AS.clip = newClipToPlay;
        AS.Play();
    }

    public void PlayAudio(AudioType type)
    {
        NewClip(Enemy_AudioClips[(int)type]);
    }

}
