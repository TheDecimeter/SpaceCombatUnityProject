using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private ControlEvents _controls;
    private bool releaseL, releaseR, releaseB, releaseS;
    public ControlEvents controls
    {
        get
        {
            return _controls;
        }
        set
        {
            if(_controls&&_controls.shade)
                _controls.shade.SetActive(false);
            _controls = value;
            if (_controls.shade)
                _controls.shade.SetActive(true);
        }
    }

    public void PassControl(ControlEvents to)
    {
        controls = to;
    }
    
    public void ControllerListener(ControlStruct controls)
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (controls.moveLeft < -.2 && releaseL)
        {
            releaseL = false;
            this.controls.FireL();
        }
        else if(controls.moveLeft > -.1)
            releaseL = true;
        if (controls.moveLeft > .2 && releaseR)
        {
            releaseR = false;
            this.controls.FireR();
        }
        else if (controls.moveLeft < .1)
            releaseR = true;
        if (controls.jump && releaseS)
        {
            releaseS = false;
            this.controls.FireS();
        }
        else
            releaseS = true;
        if (controls.door && releaseB)
        {
            releaseB = false;
            this.controls.FireB();
        }
        else
            releaseB = true;
    }

    public void FireL()
    {
        controls.FireL();
    }
    public void FireR()
    {
        controls.FireR();
    }
    public void FireS()
    {
        controls.FireS();
    }
    public void FireB()
    {
        controls.FireB();
    }
}
