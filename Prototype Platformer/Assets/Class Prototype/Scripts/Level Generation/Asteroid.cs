﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private static AudioManager audio;
    private bool playOnce = true;
    // Start is called before the first frame update
    void Start()
    {
        if (audio == null)
            audio = FindObjectOfType<AudioManager>();
    }


    public void OnCollisionEnter(Collision col)
    {
        //astroid impact sounds can go here
        //print("AsteroidHit");
        if (playOnce)
        {
            playOnce = false;
            audio.Play("AsteroidHit");
        }
    }
}
