using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIspecialDirections : MonoBehaviour
{
    public bool up = false;
    public bool down = false;
    public bool left = false;
    public bool right = false;

    public void OnTriggerEnter(Collider other)
    {
        Transform current = other.transform;
        while (current)
        {
            //print("special directions enter "+current.gameObject.name+" "+gameObject.name);
            AI ai = current.GetComponent<AI>();
            if (ai)
            {
               // print("special directions dilevered" + " " + gameObject.name);
                ai.SetDirections(up, down, left, right);
                return;
            }
            current = current.parent;
        }
        //print("special directions NOT dilevered" + " " + gameObject.name);
    }
    public void OnTriggerExit(Collider other)
    {
        Transform current = other.transform;
        while (current)
        {
            //print("special directions exit " + other.gameObject.name + " " + gameObject.name);
            AI ai = current.GetComponent<AI>();
            if (ai)
            {
                //print("special directions removed" + " " + gameObject.name);
                ai.RemoveDirections();
                return;
            }
            current = current.parent;
        }
    }

}
