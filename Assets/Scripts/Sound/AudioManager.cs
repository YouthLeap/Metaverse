using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void Play(string audioname)
    {
        foreach (Sound s in sounds)
        {
            if(s.name==audioname)
            {
                s.source.volume = s.volume * PlayerPrefs.GetFloat("EffectAudio", 1.0f) * PlayerPrefs.GetFloat("MasterAudio", 1.0f);
                s.source.Play();
                break;
            }
        }
    }

    public void Playcontinue(string audioname, string[] except = default)
    {

        bool shouldplay = true;


        if(except!=default)
        foreach (Sound s in sounds)
        {
            foreach(string str in except)
            {
                if (s.name == str)
                {
                    shouldplay = false;
                    break;
                }
            }
            
        }

        if(shouldplay)
        foreach (Sound s in sounds)
        {
            if (s.name == audioname)
            {
                s.source.volume = s.volume * PlayerPrefs.GetFloat("EffectAudio", 1.0f) * PlayerPrefs.GetFloat("MasterAudio", 1.0f);
                if(!s.source.isPlaying)
                    s.source.Play();
                break;
            }
        }
    }

    public void Stop(string audioname)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == audioname)
            {
                s.source.Stop();
                break;
            }
        }
    }

    public void AdjustVolume(string audioname,float volume)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == audioname)
            {
                s.volume = volume;
                break;
            }
        }
    }

    public void PlayButtonClick()
    {
        Play("MR_button_click");
    }

    public void PlayButtonHover()
    {
        Play("MR_Hover_click");
    }

    public void PlayLogin(float volume)
    {
        AdjustVolume("MR_LOGIN_SOUND", volume);
        Play("MR_LOGIN_SOUND");
    }

    public void PlayMessageReceived()
    {
        Play("MR_MessageReceive");
    }

    public void PlayMessageSend()
    {
        Play("MR_MessageSent");
    }
}
