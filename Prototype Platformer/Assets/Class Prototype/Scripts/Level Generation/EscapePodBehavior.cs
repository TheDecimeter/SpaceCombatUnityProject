using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EscapePodBehavior : MonoBehaviour
{
    public string targetTag = "Player";
    public int LaunchSpeed = 10;
    public int AnimationFrames = 10;
    public int ImpactDamage=10;
    public UnityEvent EscapeEvent;
    private bool dontMove = false;
    private GameObject player = null;
    private int frameCounter;
    
    void FixedUpdate()
    {
        if (player != null)
        {
            if (frameCounter++ == AnimationFrames)
            {
                leave();
                player.SetActive(false);
                FindObjectOfType<UndestroyableData>().
                    IncreaseScore(player.GetComponent<CharacterMovement_Physics>().
                    PlayerNumber,1);
            }
            else
            {
                player.transform.localScale = new Vector3(1, 
                    (AnimationFrames-frameCounter)/AnimationFrames, 1);
            }
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
            player = collision.gameObject;
            player.GetComponent<CharacterMovement_Physics>().Freeze(true);
            frameCounter = 0;
        }
    }

    void leave()
    {
        transform.LookAt(player.GetComponent<CameraLocator>().CameraLocation);
        GetComponent<Rigidbody>().velocity = transform.forward * LaunchSpeed;
        EscapeEvent.Invoke();
    }
}
