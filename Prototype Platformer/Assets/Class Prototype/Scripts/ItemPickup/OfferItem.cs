using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class OfferItem : MonoBehaviour
{
    private string offerItemMethod="encounteredItem";
    public string targetTag = "Player";
    public float setz = -0.2f;
    public Item ItemType;
    public GameObject highlight;

    private bool itemHasBeenGotten;

    public Collider _childCollider;
    // Start is called before the first frame update
    void Start()
    {
        highlight= transform.Find("Pointer").gameObject;

        itemHasBeenGotten = false;
        if(_childCollider==null)
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
        highlight.SetActive(!response);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == targetTag)
        {
            Physics.IgnoreCollision(collision.collider, _childCollider);
        }
        else
        {
            //transform.position = new Vector3(
            //    transform.position.x,
            //    transform.position.y,
            //    setz);
        }
    }
}
