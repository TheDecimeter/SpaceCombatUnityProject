using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EscapePodBehavior : MonoBehaviour
{
    public string targetTag = "Player";
    public int LaunchSpeed = 10;
    public int AnimationFrames = 5;
    public int ImpactDamage = 10;
    public UnityEvent EscapeEvent;
    private bool dontMove = false;
    private GameObject player = null;
    private int frameCounter;
    private GameObject PlayerArray;
    public float DistanceToEnter = 3;
    
    public Vector3 AsyncPosition { get; protected set; }
    public void Init()
    {
        AsyncPosition = transform.position;
    }


    public void ShowNavPoints()
    {
        PlayerArray = GameObject.Find("/Players/PlayerArray");
        foreach (Transform child in PlayerArray.transform)
            if (!child.gameObject.GetComponent<PlayerHealth>().isDead)
                child.gameObject.GetComponent<CharacterMovement_Physics>().PointTo(transform);
    }

    void FixedUpdate()
    {
        AsyncPosition = transform.position;
        if (player != null)
        {
            if (frameCounter++ == AnimationFrames)
            {
                leave();
                player.SetActive(false);
                player.GetComponent<CharacterMovement_Physics>().ClearNavPoints();
                FindObjectOfType<UndestroyableData>().
                    IncreaseScore(player.GetComponent<CharacterMovement_Physics>().
                    PlayerNumber, 1);
            }
            else
            {
                player.transform.localScale = new Vector3(1,Mathf.Lerp(1,0,frameCounter / (float)AnimationFrames), 1);
            }
        }
        else
        {
            checkPlayerDistance();
            if (dontMove)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (player != null)
            return;

        if (!dontMove && collision.gameObject.tag == targetTag)
        {
            collision.gameObject.GetComponent<PlayerHealth>().DealDamage(new DamageMessage(ImpactDamage, gameObject));
        }

        dontMove = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;


        if (dontMove && collision.gameObject.tag == targetTag)
        {
            if (collision.gameObject.GetComponent<PlayerHealth>().isDead)
            {
                return;
            }
            if (player == null)
            {
                SetPlayer(collision.gameObject);
            }
        }
    }

    void leave()
    {
        PlayerHealth.StopDamage = true;
        dontMove = false;
        Vector3 head=PointAt(player.GetComponent<CameraLocator>().CameraLocation.position);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = head * LaunchSpeed;
        if(rb.angularVelocity==Vector3.zero)
        {
            float x = Random.Range(0, 1);
            float y = 1 - x;
            float z = Random.Range(0, 1);
            if (Random.Range(0, 2) > 0)
                x *= -1;
            if (Random.Range(0, 2) > 0)
                y *= -1;
            if (Random.Range(0, 2) > 0)
                z *= -1;
            rb.AddTorque(new Vector3(x, y, z) * 100);
        }
        EscapeEvent.Invoke();
    }

    Vector3 PointAt(Vector3 loc)
    {
        return (loc - transform.position).normalized;
    }

    void checkPlayerDistance()
    {
        List<GameObject> players = null;
        foreach (Transform child in PlayerArray.transform)
            if (distance(transform.position, child.position) < DistanceToEnter)
            {
                if (child.gameObject.GetComponent<PlayerHealth>().isDead)
                    continue;

                if (players == null)
                    players = new List<GameObject>();
                players.Add(child.gameObject);
            }
        if (players!=null){
            SetPlayer(players[Random.Range(0, players.Count)]);
        }
    }

    private void SetPlayer(GameObject player)
    {
        if (this.player != null)
            return;
        this.player = player;
        FreezePlayer(player);
        frameCounter = 0;
    }

    private static float distance(Vector3 from, Vector3 to)
    {
        return Mathf.Sqrt(Mathf.Pow(from.x - to.x, 2) + Mathf.Pow(from.y - to.y, 2));
    }

    private void FreezePlayer(GameObject player)
    {
        CharacterMovement_Physics c = player.GetComponent<CharacterMovement_Physics>();
        c.Freeze(true);
        c.BecomeSticky();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
}
