using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    //public AudioClip Clips;
    private AudioSource player;
    //private UndestroyableData savedData;
    public AudioSource music;
    private static AudioManager master = null;
    private Dictionary<string, Sound> soundTable;

    void Awake() 
    {
        if (master != null)
        {
            Destroy(music);//since music plays constantly, destroy new instances which try to play it
            Destroy(GetComponent<AudioListener>());
        }
        else
        {
            soundTable = new Dictionary<string, Sound>();
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;

                soundTable.Add(s.name, s);
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
                Sound s = soundTable[name];// Array.Find(sounds, sound => sound.name == name);
                //if(name.Contains("Death")) s.source.Play();
                s.source.PlayOneShot(s.clip, s.volume);
            }
            else
                master.Play(name);
        }
    }

    public static void PlayFast()
    {
        if (master == null)
            return;

        master.StopAllCoroutines();
        master.StartCoroutine(master.ChangeSpeed(1.02f));
    }

    IEnumerator ChangeSpeed(float goal)
    {
        float initialPitch = master.music.pitch;
        if (initialPitch != goal)
        {
            float count = 0;
            while (count < 1)
            {
                count += Time.deltaTime;
                master.music.pitch = Mathf.Lerp(initialPitch, goal, count);
                yield return null;
            }
        }
    }

    public static void PlayNormal()
    {
        if (master == null)
            return;

        master.StopAllCoroutines();
        master.StartCoroutine(master.ChangeSpeed(1f));
    }

    private float Vol(string name,Sound s)
    {
        if (name.Contains("Hurt"))
            return UndestroyableData.GetTrueSFXGruntVolume()*UndestroyableData.GetTrueSFXMasterVolume()*s.volume;
        return UndestroyableData.GetTrueSFXMasterVolume() * s.volume;
    }
}
