using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private ControlEvents _controls;
    private bool releaseL, releaseR, releaseB, releaseS;
    private ControlStruct _controllerStatus = new ControlStruct(ControlStruct.None);
    
    private void Start()
    {
        enabled=(FindObjectOfType<UndestroyableData>().isMenuOpened());
    }

    public ControlEvents controls
    {
        get
        {
            return _controls;
        }
        set
        {
            if (_controls)
            {
                _controls.Active = false;
                if(_controls.HideIfNoFocus&&_controls.MenuChild!=value)
                    _controls.HideIfNoFocus.SetActive(false);
            }
            _controls = value;
            _controls.Active = true;

            if (_controls.HideIfNoFocus)
                _controls.HideIfNoFocus.SetActive(true);
        }
    }

    public void PassControl(ControlEvents to)
    {
        controls = to;
    }

    public void PassControl(ScrollManager to)
    {
        controls = to.Controls[to.atControl];
    }

    public void ControllerListener(ControlStruct newControls)
    {

        if (!gameObject.activeInHierarchy || !enabled)
            return;

        //if (_controllerStatus.fromSource(newControls.source))
        //{
        //    print("resetting controls " + newControls.source);
        //    _controllerStatus = newControls;
        //}
        //else
            _controllerStatus.combine(newControls);

        if (_controllerStatus.moveLeft < -.2)
        {
            if (releaseL)
            {
                releaseL = false;
                this.controls.FireL();
            }
        }
        else if (_controllerStatus.moveLeft > -.1)
        {
            //print("reset L " + _controllerStatus.moveLeft+" "+_controllerStatus.source);
            releaseL = true;
        }
        if (_controllerStatus.moveLeft > .2)
        {
            if (releaseR)
            {
                releaseR = false;
                this.controls.FireR();
            }
        }
        else if (_controllerStatus.moveLeft < .1)
        {
            releaseR = true;
        }
        if (_controllerStatus.jump)
        {
            if (releaseS)
            {
                releaseS = false;
                this.controls.FireS();
            }
        }
        else
            releaseS = true;
        if (_controllerStatus.B)
        {
            if (releaseB)
            {
                releaseB = false;
                this.controls.FireB();
            }
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
