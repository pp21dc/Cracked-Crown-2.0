using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushAudio : MonoBehaviour
{
    [SerializeField]
    List<AudioSource> bushHit_ASs;
    [SerializeField]
    List<AudioClip> bush_Clips;

    private void PlayBushHit()
    {
        foreach(AudioSource AS in bushHit_ASs)
        {
            if (!AS.isPlaying)
            {
                AS.clip = bush_Clips[Random.Range(0, bush_Clips.Count)];
                AS.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            PlayBushHit();
    }

}
