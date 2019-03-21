using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    //public AudioClip Clips;
    private AudioSource player;
    private UndestroyableData savedData;

    public float MenuVolume = .2f;
    public float GameVolume = 1f;
    // Start is called before the first frame update
    void Awake() 
    {
        player = gameObject.AddComponent<AudioSource>();
        savedData = FindObjectOfType<UndestroyableData>();
        if (savedData.isMenuOpened())
            player.volume = MenuVolume;
        else
            player.volume = GameVolume;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play (string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name);
        if(name.Contains("Death")) s.source.Play();
        //s.source.PlayOneShot(s.clip);
        else player.PlayOneShot(s.clip);
    }
}
