using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPointer : MonoBehaviour
{
    public float OrbitRadius = 3;
    public Transform Mesh;

    Transform target = null;
    Transform reference = null;
    Transform cam = null;
    float offset,rotation=0;

    public void Init(Transform target, Transform reference, float offset, Transform camera)
    {
        this.target = target;
        this.reference = reference;
        this.offset = offset;
        gameObject.SetActive(true);
        this.cam = camera;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        
        transform.position = center();
        //gameObject.transform.position = reference.position;// + offset;
        gameObject.transform.LookAt(target);
        gameObject.transform.position += gameObject.transform.forward * OrbitRadius;

        //transform.up = Vector3.up;
        //Mesh.right = Vector3.forward;
        //rotation += Time.deltaTime * 10;
        //if (rotation > 360)
        //    rotation -= 360;
        Mesh.RotateAround(Mesh.position, Mesh.right, Time.deltaTime * 60);

    }

    private Vector3 center()
    {
        Vector3 ret = cam.position-reference.position;
        //Debug.DrawLine(cam.position, reference.position);
        return reference.position + (ret.normalized * offset);
    }
}
