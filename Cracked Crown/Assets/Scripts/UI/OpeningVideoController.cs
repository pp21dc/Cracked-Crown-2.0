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
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < players.Length; i++)
        {
            players[i].Prepare();
            
        }

    }

    public void PlayVideo()
    {
        ResetVideoPlayer();
        GameManager.Instance.waitforvideo = true;
        players[0].Play();
        active = true;
    }

    void ResetVideoPlayer()
    {
        for(int i = 0; i < players.Length; i++) 
        {
            players[i].enabled = true;
            players[i].Prepare();
            j = 0;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Debug.LogWarning("VideoControllerUpdate");
            //Looks to see if a key has been pressed to skip to next video
            if (Input.anyKeyDown && j < players.Length)
            {
                players[j].enabled = false;
                j++;
                if (j < players.Length)
                    players[j].Play();
                else
                {
                    players[2].enabled = false;
                    GameManager.Instance.waitforvideo = false;
                    active = false;
                    GameManager.Instance.FreezePlayers(false);
                }
            }
            else if (Input.anyKeyDown && j >= players.Length)
            {
                players[2].enabled = false;
                GameManager.Instance.waitforvideo = false;
                active = false;
                GameManager.Instance.FreezePlayers(false);
            }
            //checks if first video is finished and starts looping seccond video (showing text with anim)
            if (player.isPaused)
            {
                if (j == 0)
                {
                    players[j].enabled = false;

                    j++;

                    players[j].Play();
                    players[j].isLooping = true;
                }
            }

            //Closes the video player setup once the 3rd video is done
            if ((j == 2 && players[2].isPaused))
            {
                players[2].enabled = false;
                GameManager.Instance.waitforvideo = false;
                active = false;
                GameManager.Instance.FreezePlayers(false);
            }
        }
    }
}
