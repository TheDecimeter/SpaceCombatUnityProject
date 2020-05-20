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

    private const int itemMask= (1 << 18) ;

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
                Debug.LogWarning("Diverting asteroid");
                float x = Random.Range(0, 1);
                float y = 1 - x;
                if (Random.Range(0, 2) > 0)
                    x *= -1;
                if (Random.Range(0, 2) > 0)
                    y *= -1;
                Vector2 v = new Vector2(x, y);
                //Rigidbody rb = GetComponent<Rigidbody>();
                v = v.normalized * 4000;//rb.velocity.normalized;
                divert = false;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
                rb.AddForce(new Vector3(v.x, v.y, 0));
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
            InteractWithEnvironment();
        }
    }

    private void InteractWithEnvironment()
    {
        if (AI.StaticLevelStats == null)
            return;
        float radius = Mathf.Max(AI.StaticLevelStats.xTileSize, AI.StaticLevelStats.yTileSize);
        int x, y;
        AI.GetMapGridPos(transform.position, out x, out y);

        PropelSurroundingObjects(x,y, radius*2);
    }

    private void PropelSurroundingObjects(int x, int y, float radius)
    {
        foreach(Collider c in Physics.OverlapSphere(transform.position, radius, itemMask, QueryTriggerInteraction.Ignore))
        {
            int cx, cy;
            AI.GetMapGridPos(c.bounds.center, out cx, out cy);
            if (cx == x && cy == y)
            {
                Rigidbody rb;
                rb = c.transform.parent.GetComponent<Rigidbody>();

                if (!rb)
                {
                    Debug.LogError("unexpected object " + c.transform.parent.name);
                    continue;
                }

                MakeSlippery(c);

                //float rot = 30f;
                float xythrust = 1f;
                float zthrust = 20f;
                rb.useGravity = false;
                rb.constraints = 0;
                rb.AddForce(new Vector3(Random.Range(-xythrust, xythrust), Random.Range(-xythrust, xythrust), zthrust));
                //rb.AddTorque(new Vector3(Random.Range(-rot, rot), Random.Range(-rot, rot), Random.Range(-rot, rot)));
            }
        }
    }

    private void MakeSlippery(Collider c)
    {
        c.material = new PhysicMaterial();
        c.material.dynamicFriction = 0;
        c.material.staticFriction = 0;
        c.material.frictionCombine = PhysicMaterialCombine.Minimum;
    }
}
