using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameObject : MonoBehaviour {


    public GameObject _Target;
    public Vector3 _Destination;
    public bool _ResetVelocity = true;

    public void MoveTargetToPosition()
    {
        _Target.transform.position = _Destination;
        if (_ResetVelocity && _Target.GetComponent<Rigidbody>() != null) _Target.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
