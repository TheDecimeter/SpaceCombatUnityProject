using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    private float maxBob = .2f, minBob = 0, bobRate = .01f, currentBob;
    // Start is called before the first frame update
    void Start()
    {
        currentBob = minBob;
    }

    // Update is called once per frame
    void Update()
    {
        currentBob += bobRate;
        if (currentBob > maxBob||currentBob<minBob) bobRate *= -1;
        transform.forward = Vector3.forward;
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y + currentBob, transform.parent.position.z);
    }
}
