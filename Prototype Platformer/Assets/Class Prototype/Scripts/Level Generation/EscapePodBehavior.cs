using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EscapePodBehavior : MonoBehaviour
{
    public string targetTag = "Player";
    public int LaunchSpeed = 10;
    public int AnimationFrames = 10;
    public int ImpactDamage = 10;
    public UnityEvent EscapeEvent;
    private bool dontMove = false;
    private GameObject player = null;
    private int frameCounter;
    private GameObject PlayerArray;
    public float DistanceToEnter = 3;
    

    public void ShowNavPoints()
    {
        PlayerArray = GameObject.Find("/Players/PlayerArray");
        foreach (Transform child in PlayerArray.transform)
            if (!child.gameObject.GetComponent<PlayerHealth>().isDead)
                child.gameObject.GetComponent<CharacterMovement_Physics>().PointTo(transform);
    }

    void FixedUpdate()
    {

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
                player.transform.localScale = new Vector3(1,
                    (AnimationFrames - frameCounter) / AnimationFrames, 1);
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
                player = collision.gameObject;
                FreezePlayer(player);
                frameCounter = 0;
            }
        }
    }

    void leave()
    {
        dontMove = false;
        transform.LookAt(player.GetComponent<CameraLocator>().CameraLocation);
        GetComponent<Rigidbody>().velocity = transform.forward * LaunchSpeed;
        EscapeEvent.Invoke();
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
            player = players[Random.Range(0, players.Count)];
            FreezePlayer(player);
            frameCounter = 0;
        }
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
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }
}
