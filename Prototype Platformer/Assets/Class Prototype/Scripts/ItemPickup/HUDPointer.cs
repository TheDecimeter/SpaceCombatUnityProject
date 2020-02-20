using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPointer : MonoBehaviour
{
    public float OrbitRadius = 3;

    Transform target = null;
    Transform reference = null;
    Transform cam = null;
    Vector3 offset;

    public void Init(Transform target, Transform reference, Vector3 offset, Transform camera)
    {
        this.target = target;
        this.reference = reference;
        this.offset = offset;
        gameObject.SetActive(true);
        this.cam = camera;
        print("init pointer");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        

        if (cam == null)
        {
            gameObject.transform.position = reference.position + offset;
            gameObject.transform.LookAt(target);
            gameObject.transform.position += gameObject.transform.forward * OrbitRadius;

        }

    }
}
