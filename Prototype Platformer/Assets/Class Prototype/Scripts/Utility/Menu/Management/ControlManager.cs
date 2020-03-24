using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private ControlFirer _controls;
    private bool releaseL, releaseR, releaseB, releaseS;
    public ControlFirer initialMenuItem;
    public bool IsTitleMenu = true;

    private ControlListener c1, c2, c3, c4;
    

    public ControlFirer controls
    {
        get
        {
            return _controls;
        }
        set
        {
            //print("switching from " + _controls.name + " to " + value.name);
            _controls.Active = false;
            if (!_controls.HasChild(value))
            {
                //print("not child; " + value.name);
                _controls.SetHide(false);
            }
            
            _controls = value;
            _controls.Active = true;

            _controls.SetHide(true);
        }
    }

    private void Awake()
    {
        Init();
        //c1 = new ControlListener(this);
        //c2 = new ControlListener(this);
        //c3 = new ControlListener(this);
        //c4 = new ControlListener(this);

        //_controls = initialMenuItem;
        //controls = initialMenuItem;
        if(IsTitleMenu)
            enabled = (FindObjectOfType<UndestroyableData>().isMenuOpened());
    }

    public void Init()
    {
        if (c1 == null)
        {
            c1 = new ControlListener(this);
            c2 = new ControlListener(this);
            c3 = new ControlListener(this);
            c4 = new ControlListener(this);

            _controls = initialMenuItem;
            controls = initialMenuItem;
        }
    }

    public void PassControl(ControlFirer to)
    {
        controls = to;
        while(to is ScrollManager)
        {
            ScrollManager s = (ScrollManager)to;
            to = s.Controls[s.atControl];
            controls = to;
        }
    }

    public void ControllerListener1(ControlStruct newControls)
    {
        if (gameObject.activeInHierarchy && enabled)
        {
            //print("control listener, performing action " + gameObject.name);
            c1.ControllerListener(newControls);
        }
    }
    public void ControllerListener2(ControlStruct newControls)
    {
        if (gameObject.activeInHierarchy && enabled)
            c2.ControllerListener(newControls);
    }
    public void ControllerListener3(ControlStruct newControls)
    {
        if (gameObject.activeInHierarchy && enabled)
            c3.ControllerListener(newControls);
    }
    public void ControllerListener4(ControlStruct newControls)
    {
        if (gameObject.activeInHierarchy && enabled)
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
