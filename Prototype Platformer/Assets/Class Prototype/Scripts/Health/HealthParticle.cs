using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthParticle : MonoBehaviour
{
    private Vector3 velocity, gravity;
    private float timeout;

    // Update is called once per frame
    void Update()
    {
        velocity += gravity*Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        timeout -= Time.deltaTime;
        if (timeout <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void Fire(Vector3 launchVelocity, int health, float timeToLive)
    {
        if (health == 0)
        {
            Destroy(gameObject);
            return;
        }
        //TextMeshPro text = gameObject.AddComponent<TextMeshPro>();
        if (health > 0)
        {
            //text.faceColor = Color.green;
            //gravity = Vector3.zero;
            //text.text="+"+health;
            Fire(launchVelocity, Vector3.zero, "+" + health, Color.green, timeToLive);
        }
        else
        {
            //text.faceColor = Color.red;
            //gravity = Physics.gravity;
            //text.text = "" + health;
            Fire(launchVelocity, Physics.gravity, "" + health, Color.red, timeToLive);
        }
        //text.alignment = TextAlignmentOptions.Bottom;
        //text.fontSize = 8;

        //timeout = timeToLive;
        //velocity = launchVelocity;
    }


    public void Fire(Vector3 launchVelocity, Vector3 gravity, string message, Color color, float timeToLive)
    {
        TextMeshPro text = gameObject.AddComponent<TextMeshPro>();
        
        text.faceColor = color;
        text.text = message;
        
        text.alignment = TextAlignmentOptions.Bottom;
        text.fontSize = 8;

        timeout = timeToLive;
        velocity = launchVelocity;
        this.gravity = gravity;
    }

    public static void Create(Transform loc, int health)
    {
        GameObject g = new GameObject("HealthParticle");
        HealthParticle h = g.AddComponent<HealthParticle>();

        g.transform.position = new Vector3(loc.position.x,loc.position.y+3,loc.position.z);

        float X=Random.Range(-.5f, .5f);
        float Z= Random.Range(0, .5f);
        Vector3 LaunchVel = new Vector3(X, 0.5f, Z);

        float timeout = Random.Range(.5f, 2);
        h.Fire(LaunchVel.normalized*Physics.gravity.magnitude/4, health, timeout);
    }

    public static void Create(Transform loc, string message, Color color, bool rise)
    {
        GameObject g = new GameObject("HealthParticle");
        HealthParticle h = g.AddComponent<HealthParticle>();

        g.transform.position = new Vector3(loc.position.x, loc.position.y + 3, loc.position.z);

        float X = Random.Range(-.5f, .5f);
        float Z = Random.Range(0, .5f);
        Vector3 LaunchVel = new Vector3(X, 0.5f, Z);

        float timeout = Random.Range(.5f, 2);

        Vector3 gravity;
        if (rise)
             gravity = Vector3.zero;
        else
            gravity = Physics.gravity;

        h.Fire(LaunchVel.normalized * Physics.gravity.magnitude / 4, gravity , message,color, timeout);
    }

    public static void Create(Transform loc, Vector3 dir, float life, string message, Color color, bool rise)
    {
        GameObject g = new GameObject("HealthParticle");
        HealthParticle h = g.AddComponent<HealthParticle>();

        g.transform.position = new Vector3(loc.position.x, loc.position.y + 3, loc.position.z);

        Vector3 gravity;
        if (rise)
            gravity = Vector3.zero;
        else
            gravity = Physics.gravity;

        h.Fire(dir, gravity, message, color, life);
    }
}
