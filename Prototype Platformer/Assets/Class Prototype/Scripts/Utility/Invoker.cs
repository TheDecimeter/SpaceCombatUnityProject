using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Invoker : MonoBehaviour {

    public UnityEvent _Event;
    public float _StartDelay;
    public float _InvokeDelay;
    public bool _StartActive;

	void Start () 
    {
        if (_StartActive) BeginInvoking();
	}
	
    public void BeginInvoking ()
    {
        InvokeRepeating("InvokeEvent", _StartDelay, _InvokeDelay);
    }

    public void InvokeEvent ()
    {
        _Event.Invoke();
    }
}
