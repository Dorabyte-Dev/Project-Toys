using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Sound[] sounds;
    public Sound currentTheme;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Play(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        if (s.type == Sound.SoundType.music) currentTheme = s;
        s.source.pitch = pitch;
        s.source.Play();
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        if (s.type == Sound.SoundType.music) currentTheme = s;
        s.source.Play();
    }
    public void PlayRandomInRange(string[] names)
    {
        Sound[] _sounds = new Sound[names.Length];
        
        for(int i = 0; i < names.Length; i++)
        {
            Sound s = Array.Find(sounds, sound => sound.name == names[i]);
            _sounds[i] = s;
            if(s == null)
            {
                Debug.LogWarning("Sound with name: " + names[i] + "not found");
            }
        }

        int rand = UnityEngine.Random.Range(0, _sounds.Length);

        _sounds[rand].source.Play();

    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Stop();
    }
    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Pause();
    }
    public IEnumerator Pause(string name, float time)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) yield break;
        s.source.Pause();
        yield return new WaitForSecondsRealtime(time);
        s.source.UnPause();
    }
    public void Unpause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.UnPause();
    }
    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.volume = volume;
    }
    public void SetVolumeToZero(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.volume = 0;
    }
    public Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null) return s;
        else return null;
    }
    //Fade out

    public void FadeOut(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.DOFade(0, 4).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            Stop(name);
            SetVolume(name, s.volume);

        });
    }
    public void FadeIn(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        SetVolume(name, 0);
        s.source.DOFade(s.volume, 4).SetEase(Ease.InOutSine);
    }
    public void StopAllSfx()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying && s.type == Sound.SoundType.sfx)
            {
                s.source.Stop();
            }
        }
    }
    public void StopAllMusic()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying && s.type == Sound.SoundType.music)
            {
                s.source.Stop();
            }
        }
    }
    public void StopAllSounds()
    {
        foreach(Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }

    public void ChangeCurrentTheme(string name)
    {
        FadeOut(currentTheme.name);
        Play(name);
        FadeIn(name);
    }
}
