using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public List<AudioClip> Tracks;
    public AudioSource AS_Soundtrack;
    public float FadeTime = 0.25f;
    public float PlayDelayTime = 1;
    public int trackIndex = -1;

    public static MusicManager instance;

    private void Awake()
    {
        instance = this;
    }

    public enum TrackTypes
    {
        intro,
        windy,
        loading,
        shop,
        room1,
        room2,
        room3,
        boss
    }

    public void PlayNextTrack()
    {
        //Debug.Log("AUDIO");
        StopAllCoroutines();
        StartCoroutine(FadeToNext(trackIndex+1));
    }

    public void PlayTrack(TrackTypes track)
    {
        trackIndex = (int)track;
        StopAllCoroutines();
        StartCoroutine(FadeToNext(trackIndex));
    }


    public void PlayNextTrack(bool instant)
    {
        //AS_Soundtrack.volume = 0;
        trackIndex++;
        AS_Soundtrack.clip = Tracks[trackIndex];
        AS_Soundtrack.Play();
    }

    IEnumerator FadeToNext(int trackTo)
    {
        while (true)
        {
            AS_Soundtrack.volume -= FadeTime * Time.deltaTime;
            if (AS_Soundtrack.volume <= 0.01f)
                break;
            yield return new WaitForEndOfFrame();
        }
        AS_Soundtrack.Stop();
        AS_Soundtrack.clip = Tracks[trackTo];
        AS_Soundtrack.Play();
        while (true)
        {
            AS_Soundtrack.volume += FadeTime * Time.deltaTime;
            if (AS_Soundtrack.volume >= 0.2f)
                break;
            yield return new WaitForEndOfFrame();
        }
        
        
        
        yield return null;
    }

}
