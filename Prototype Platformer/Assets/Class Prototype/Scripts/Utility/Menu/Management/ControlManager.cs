using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private ControlEvents _controls;
    private bool releaseL, releaseR, releaseB, releaseS;
    private ControlStruct _controllerStatus = new ControlStruct(ControlStruct.None);

    private ControlListener c1, c2, c3, c4;
    

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

    private void Start()
    {
        c1 = new ControlListener(this);
        c2 = new ControlListener(this);
        c3 = new ControlListener(this);
        c4 = new ControlListener(this);

        enabled = (FindObjectOfType<UndestroyableData>().isMenuOpened());
    }
    public void PassControl(ControlEvents to)
    {
        controls = to;
    }

    public void PassControl(ScrollManager to)
    {
        controls = to.Controls[to.atControl];
    }

    public void ControllerListener1(ControlStruct newControls)
    {
        c1.ControllerListener(newControls);
    }
    public void ControllerListener2(ControlStruct newControls)
    {
        c2.ControllerListener(newControls);
    }
    public void ControllerListener3(ControlStruct newControls)
    {
        c3.ControllerListener(newControls);
    }
    public void ControllerListener4(ControlStruct newControls)
    {
        c4.ControllerListener(newControls);
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
