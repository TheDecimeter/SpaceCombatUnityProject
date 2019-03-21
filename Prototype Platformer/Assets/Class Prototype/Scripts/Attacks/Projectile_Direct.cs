using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile_Direct : Projectile 
{
    public float projectileSpeed = 10f;
    public float timeToDie = 5f;

    private Rigidbody _rigidbody;

    public GameObject friend;

    public void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();

        StartCoroutine(Timout());
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
            print("colided with frriend " + collision.gameObject.name);
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
                print("colided with frriend's parent " + collision.gameObject.name);
                return;
            }
        }
        foreach (Transform child in friend.transform)
        {
            if (child.gameObject == collision.collider.gameObject)
            {
                print("colided with frriend " + collision.gameObject.name);
                return;
            }
        }
        print("   colided with NON FRIEND"+collision.gameObject.name);

        //print("destroyed due to colission "+ collision.collider.ToString());
        Destroy(this.gameObject);
    }

    IEnumerator Timout ()
    {
        yield return new WaitForSeconds(timeToDie);

        Destroy(this.gameObject);
    }

}
