using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class SoundController : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] themes;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

        foreach (Sound s in themes)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = true;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayButton()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "button");
        s.source.Play();
    }

    public void PlayBack()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "back");
        s.source.Play();
    }

    public void PlayUnavailable()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "unavailable");

        if (!s.source.isPlaying)
            s.source.Play();
    }

    public void PlayPlayerMove()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "playerMove");

        if (!s.source.isPlaying)
            s.source.Play();
    }

    public void PlayDeath()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "death");
        s.source.Play();
    }

    public void PlayMoney()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "money");
        s.source.Play();
    }

    public void PlayCapture()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "capture");
        s.source.Play();
    }

    //themes
    public void PlayCani()
    {
        StopHipster();

        Sound s = Array.Find(themes, sound => sound.name == "cani");
        s.source.Play();
    }

    public void StopCani()
    {
        Sound s = Array.Find(themes, sound => sound.name == "cani");
        s.source.Stop();
    }

    public void PlayHipster()
    {
        StopCani();

        Sound s = Array.Find(themes, sound => sound.name == "hipster");
        s.source.Play();
    }

    public void StopHipster()
    {
        Sound s = Array.Find(themes, sound => sound.name == "hipster");
        s.source.Stop();
    }

    public void PlayTitle()
    {
        StopCani();

        Sound s = Array.Find(themes, sound => sound.name == "title");
        s.source.Play();
    }

    public void StopTitle()
    {
        Sound s = Array.Find(themes, sound => sound.name == "title");
        s.source.Stop();
    }
}
