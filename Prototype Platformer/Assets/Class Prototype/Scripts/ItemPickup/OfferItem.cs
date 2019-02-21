using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Item))]
public class OfferItem : MonoBehaviour
{
    private string offerItemMethod="encounteredItem";
    public string targetTag = "Player";
    public Item ItemType;

    private bool itemHasBeenGotten;

    private Collider _childCollider;
    // Start is called before the first frame update
    void Start()
    {
        itemHasBeenGotten = false;
        _childCollider=this.transform.GetChild(0).GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnTriggerEnter(Collider other)
    {
        //print("item offered");
        if (other.CompareTag(targetTag)&&!itemHasBeenGotten)
        {
            //print("is a " + targetTag);
            other.SendMessageUpwards(offerItemMethod, this.ItemType, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    public void feedback(bool response)
    {
        itemHasBeenGotten = response;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == targetTag)
        {
            Physics.IgnoreCollision(collision.collider, _childCollider);
        }
    }
}
