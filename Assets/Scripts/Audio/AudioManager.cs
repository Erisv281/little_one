using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
// Play sounds using either
// (1) FindObjectOfType<AudioManager>().Play("NAME"); where "NAME" represents sound name
// (2) AudioManager.instance.Play("NAME");
/// </summary>
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public AudioMixerGroup audioMixer;      // Then we can click and add the master audio mixer
    public Sound[] sounds;  // Array of sounds


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Each song in the array will receive these markers for the user to fill in. 
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = audioMixer;    // Put this song into the audio mixer
        }
    }

    private void Start()
    {
        AudioSource[] m_sounds = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
    }

    public void OnEnable()
    {
        // Subscribe to events
        // Add listeners when playing certain SFX
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);  // Find the song and play it. 
        if (s == null)
        {
            print("Sound not found");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            print("Sound not found");
            return;
        }
        s.source.Stop();
    }

    public void ChangePitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            print("Sound not found");
            return;
        }
        s.source.pitch = pitch;

    }

    /// <summary>
    /// Reset all current sound
    /// </summary>
    public void Reset()
    {
        AudioSource[] m_sounds = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];

        foreach (AudioSource sound in m_sounds)
        {
            sound.Stop();

        }


    }


}