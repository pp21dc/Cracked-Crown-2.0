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

    public void PlayNextTrack()
    {
        StartCoroutine(FadeToNext());
    }

    IEnumerator FadeToNext()
    {
        trackIndex++;
        while (true)
        {
            AS_Soundtrack.volume -= FadeTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (AS_Soundtrack.volume <= 0.075f)
                break;
        }
        AS_Soundtrack.Stop();
        AS_Soundtrack.clip = Tracks[trackIndex];
        if (trackIndex == 2 || trackIndex == 4)
            yield return new WaitForSeconds(PlayDelayTime);
        AS_Soundtrack.Play();
        while (true)
        {
            AS_Soundtrack.volume += FadeTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (AS_Soundtrack.volume >= 0.2f)
                break;
        }
        
        
        
        yield return null;
    }

}
