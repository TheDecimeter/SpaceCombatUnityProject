using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    //public AudioClip Clips;
    private AudioSource player;
    //private UndestroyableData savedData;
    public AudioSource music;
    private static AudioManager master = null;

    void Awake() 
    {
        if (master != null)
        {
            Destroy(music);//since music plays constantly, destroy new instances which try to play it
            Destroy(GetComponent<AudioListener>());
        }
        else
        {
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
    }

    void Start()
    {
        if (master == null)
        {
            master = this;
            UpdateVolume();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateVolume()
    {
        if (master != null)
        {
            if (master == this)
            {
                music.volume = UndestroyableData.GetTrueMusicVolume();
                foreach (Sound s in sounds)
                {
                    s.source.volume = Vol(s.name, s);
                }
            }
            else master.UpdateVolume();
        }
        
    }
    

    public void Play (string name)
    {
        if (master != null)
        {
            if (master == this)
            {
                Sound s = Array.Find(sounds, sound => sound.name == name);
                //if(name.Contains("Death")) s.source.Play();
                s.source.PlayOneShot(s.clip, s.volume);
            }
            else
                master.Play(name);
        }
    }

    private float Vol(string name,Sound s)
    {
        if (name.Contains("Hurt"))
            return UndestroyableData.GetTrueSFXGruntVolume()*UndestroyableData.GetTrueSFXMasterVolume()*s.volume;
        return UndestroyableData.GetTrueSFXMasterVolume() * s.volume;
    }
}
