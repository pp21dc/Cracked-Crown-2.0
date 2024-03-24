using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> soundtracks;
    [SerializeField]
    private List<AudioClip> heard;
    int currentTrack;
    [SerializeField]
    private AudioSource AS_soundtrack;

    private void FixedUpdate()
    {
        NextSong();
    }

    private void NextSong()
    {
        if (AS_soundtrack.isPlaying)
            return;

        AS_soundtrack.Stop();
        heard.Add(soundtracks[currentTrack]);
        soundtracks.RemoveAt(currentTrack);
        int rr = Random.Range(0, soundtracks.Count);
        AS_soundtrack.clip = soundtracks[rr];
        AS_soundtrack.Play();
        currentTrack = rr;

    }

}
