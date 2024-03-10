using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class OpeningVideoController : MonoBehaviour
{
    [SerializeField]
    VideoClip[] videos = new VideoClip[3];

    [SerializeField]
    VideoPlayer[] players = new VideoPlayer[3];
    [SerializeField]
    VideoPlayer player;

    int j = 0;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < players.Length; i++)
        {
            players[i].Prepare();
            
        }

    }
    // Update is called once per frame
    void Update()
    {
        //Looks to see if a key has been pressed to skip to next video
        if (Input.anyKeyDown && j+1 < players.Length)
        {
            players[j].enabled = false;
            j++;
            players[j].Play();
        }
        else 
        {
            players[2].enabled = false;
            GameManager.Instance.waitforvideo = false;
        }
        //checks if first video is finished and starts looping seccond video (showing text with anim)
        if (player.isPaused)
        {
            if(j == 0)
            {
                players[j].enabled = false;

                j++;

                players[j].Play();
                players[j].isLooping = true;
            }
        }

        //Closes the video player setup once the 3rd video is done
        if((j == 2 && players[2].isPaused))
        {
            players[2].enabled = false;
            GameManager.Instance.waitforvideo = false;

        }
    }
}
