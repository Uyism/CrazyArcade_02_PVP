using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource Audio;
    public AudioClip ItemSound;
    public AudioClip BombSound;
    public AudioClip BombExplodeSound;
    public AudioClip WaterBombEndSound;

    void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    public void PlayBombAudio()
    {
        Audio.PlayOneShot(BombSound);
    }

    public void PlayBombExplodeAudio()
    {
        Audio.PlayOneShot(BombExplodeSound);
    }

    public void PlayItemAudio()
    {
        Audio.PlayOneShot(ItemSound); 
    }

    public void PlayWaterBallEndAudio()
    {
        Audio.PlayOneShot(WaterBombEndSound);
    }
}
