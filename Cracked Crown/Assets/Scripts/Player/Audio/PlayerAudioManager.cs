using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource AS;
    [SerializeField]
    private AudioSource AS2;
    [SerializeField]
    private AudioSource AS3;
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
        PC.PB.PAM = this;
        if (PC != null )
            Player_AudioClips = PC.PB.CharacterType.soundeffects;
    }

    public enum AudioType
    {
        EmptySwing,
        EnemyHit, 
        PlayerHit,
        Swing1,
        Swing2,
        Swing3,
        Swing4,
        Death,
        Move,
        Dash,
        Finisher
    }

    public AudioType[] audioTypes;

    public void NewClip(AudioClip newClipToPlay)
    {
        if (!AS.isPlaying)
        {
            AS.Stop();
            AS.pitch = Random.Range(0.98f, 1.02f);
            AS.clip = newClipToPlay;
            AS.Play();
        }
        else if (!AS2.isPlaying)
        {
            AS2.Stop();
            AS2.pitch = Random.Range(0.98f, 1.02f);
            AS2.clip = newClipToPlay;
            AS2.Play();
        }
        else if (!AS3.isPlaying)
        {
            AS3.Stop();
            AS3.pitch = Random.Range(0.98f, 1.02f);
            AS3.clip = newClipToPlay;
            AS3.Play();
        }
    }

    public void PlayAudio(AudioType type)
    {
        if (Player_AudioClips[(int)type] == null)
            return;
        if (type == AudioType.Swing1)
            NewClip(Player_AudioClips[Random.Range(3, 7)]);
        else
            NewClip(Player_AudioClips[(int)type]);
    }


}
