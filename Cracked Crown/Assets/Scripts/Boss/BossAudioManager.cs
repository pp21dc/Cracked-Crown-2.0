using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudioManager : MonoBehaviour
{
    public AudioSource[] as_boss;
    public AudioClip[] ac_boss;

    public enum AudioType
    {
        Slam,
        Roar,
        Death
    }

    private void NewClip(AudioClip newClip)
    {
        for (int i = 0; i < as_boss.Length; i++)
        {
            if (!as_boss[i].isPlaying)
            {
                as_boss[i].Stop();
                as_boss[i].clip = newClip;
                as_boss[i].Play();
            }
        }
    }

    public void PlayAudio(AudioType type)
    {
        if (ac_boss[(int)type] == null)
            return;
        NewClip(ac_boss[(int)type]);
    }

}
