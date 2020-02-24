using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public GameObject Mesh;
    public GameObject Explosion;

    private static AudioManager audio;
    private bool playOnce = true;

    private const float timeToLive = 4f;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (audio == null)
            audio = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (!playOnce)
        {
            if (timer > timeToLive)
            {
                Destroy(gameObject);
            }
            else
                timer += Time.deltaTime;
        }
    }


    public void OnCollisionEnter(Collision col)
    {
        //astroid impact sounds can go here
        //print("AsteroidHit");
        if (playOnce)
        {
            Destroy(Mesh);
            Explosion.SetActive(true);

            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


            playOnce = false;
            audio.Play("AsteroidHit");
        }
    }
}
