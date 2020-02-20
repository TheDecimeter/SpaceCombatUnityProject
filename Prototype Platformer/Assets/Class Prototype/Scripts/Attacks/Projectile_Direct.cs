using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Projectile_Direct : Projectile
{
    public float projectileSpeed = 10f;
    public float timeToDie = 5f;
    public float StayAliveAfterColission = 0f;

    private Rigidbody _rigidbody;

    public GameObject friend;

    public UnityEvent onUse;
    

    public void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();

        StartCoroutine(Timout(timeToDie));
    }

    void Update () 
    {
        this.transform.Translate(this.transform.forward * projectileSpeed * Time.deltaTime, Space.World);
	}

    public void OnCollisionEnter(Collision collision)
    {
        if (friend == collision.gameObject)
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }
        int ID = friend.GetComponent<CharacterMovement_Physics>().PlayerNumber;

        for (Transform parent = collision.gameObject.transform.parent;
            parent!=null; parent = parent.transform.parent)
        {
            //print("searching for parent " + parent.gameObject.name+ " "+ friend.name);
            if (friend == parent)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
                return;
            }
        }
        foreach (Transform child in friend.transform)
        {
            if (child.gameObject == collision.collider.gameObject)
            {
                return;
            }
        }

        onUse.Invoke();

        //print("destroyed due to colission "+ collision.collider.ToString());
        if (StayAliveAfterColission > 0)
            StartCoroutine(Timout(StayAliveAfterColission));
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    IEnumerator Timout (float time)
    {
        yield return new WaitForSeconds(time);
        
        Destroy(this.gameObject);
    }

}
