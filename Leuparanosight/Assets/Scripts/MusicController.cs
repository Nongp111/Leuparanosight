using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource bgMusic;

    public void SetVolume(float value)
    {
        bgMusic.volume = value;
    }

    public void StopMusic()
    {
        bgMusic.Stop();
    }

    public void PlayMusic(AudioClip newClip)
    {
        bgMusic.clip = newClip;
        bgMusic.Play();
    }
}
