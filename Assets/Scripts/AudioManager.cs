using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f ,1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch;

        public bool looping;

        [HideInInspector]
        public AudioSource source;
    }

    public Sound[] sounds;
    public static AudioManager instance;
    public GameObject enemyPefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.GetFloat("SoundEffectsVolume", 100) == 100)
            PlayerPrefs.SetFloat("SoundEffectsVolume", .75f);        
        if (PlayerPrefs.GetFloat("MusicVolume", 100) == 100)
            PlayerPrefs.SetFloat("MusicVolume", .75f);

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }



        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            if(s.name.Substring(0,3).Equals("Gun"))
                s.source.volume = PlayerPrefs.GetFloat("SoundEffectsVolume");
            else
                s.source.volume = PlayerPrefs.GetFloat("MusicVolume");

            
            s.source.pitch = s.pitch;
            s.source.loop = s.looping;
        }
    }

    private void Start()
    {
        Play("BackgroundMusic");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Play();
    }

    public void SetMusicVolume()
    {
        foreach(Sound s in sounds)
        {
            if (!(s.name.Substring(0, 3).Equals("Gun")))
                s.source.volume = PlayerPrefs.GetFloat("MusicVolume", .75f);
        }
    }    
    
    public void SetSoundEffectVolume()
    {
        foreach (Sound s in sounds)
        {
            if ( s.name.Substring(0, 3).Equals("Gun") )
                s.source.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
        }

        enemyPefab.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
        }
    }
}
