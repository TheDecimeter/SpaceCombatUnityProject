using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontCollideWithPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject players=FindObjectOfType<PlayerArray>().gameObject;
        foreach (Transform player in players.transform)
        {
            Physics.IgnoreCollision(player.Find("Collision/Foot Collider").gameObject.GetComponent<SphereCollider>(),
                this.GetComponent<Collider>());
            Physics.IgnoreCollision(player.Find("Collision/Body Collider").gameObject.GetComponent<CapsuleCollider>(),
                this.GetComponent<Collider>());
        }
    }
    
}
