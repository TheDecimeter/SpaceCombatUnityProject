using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public GameObject Mesh;
    public GameObject Explosion;

    private static AudioManager audio;
    private bool playOnce = true;
    public EscapePodLauncher EscapePod;
    private bool divert = true;

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
        if (divert)
        {
            if (EscapePod.AsyncIncomming)
            {
                Debug.Log("Diverting asteroid");
                float x = Random.Range(0, 1);
                float y = 1 - x;
                if (Random.Range(0, 2) > 0)
                    x *= -1;
                if (Random.Range(0, 2) > 0)
                    y *= -1;
                Vector2 v = new Vector2(x, y);
                Rigidbody rb = GetComponent<Rigidbody>();
                v =v.normalized*rb.velocity.normalized;
                divert = false;
                GetComponent<Rigidbody>().AddForce(new Vector3(v.x, v.y, 0));
            }

        }
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
