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
    //private float ignoreInput1, ignoreInput2, ignoreInput3, ignoreInput4;
    //private const float ignoreInputMargin=.9f;

    private ControlStruct ignore;
    private int ignoreAsource;

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
            //print("set controls active "+_controls.gameObject.name);
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
        if (IsTitleMenu)
        {
            enabled = (FindObjectOfType<UndestroyableData>().isMenuOpened());
        }
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

    //private void OnEnable()
    //{
    //    print("on emable");
    //    //Init();

    //    ignoreInput1 = 0;
    //    ignoreInput2 = 0;
    //    ignoreInput3 = 0;
    //    ignoreInput4 = 0;
    //}

    public void SetIgnoreController(ControlStruct c)
    {
        if (c == null)
            ignoreAsource = 0;
        else
        {
            ignoreAsource = c.ASource;
            //print("setting ignore "+c);
        }
        ignore = c;
    }

    private bool Skip(ControlStruct c)
    {
        if (ignoreAsource == 0)
            return false;
        if (ControlStruct.FromSource(ignoreAsource,c.source))
        {
            if (c.A)
            {
                //print("t I " + ControlStruct.Sources(ignoreAsource));
                //print("t C " + ControlStruct.Sources(c.ASource) + " C:" + c);
                return true;
            }
            else
            {
                //print("f I " + ControlStruct.Sources(ignoreAsource));
                //print("f C " + ControlStruct.Sources(c.ASource)+" C:"+c);
                ignoreAsource = ControlStruct.RemoveSource(c.source, ignoreAsource);
                //if (ignoreAsource == 0) { }
                //ignore = null;
                //return false;
            }
        }

        //print("l I " + ControlStruct.Sources(ignoreAsource));
        //print("l C " + ControlStruct.Sources(c.ASource) + " C:" + c);
        //if (c.A)
        //{
        //    print("but A is pressed");
        //}
            return false;
        
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
            //print("\n1");
            if (Skip(newControls))
                return;
            //print("control listener, performing action " + gameObject.name);
            c1.ControllerListener(newControls);
        }
    }
    public void ControllerListener2(ControlStruct newControls)
    {

        if (gameObject.activeInHierarchy && enabled)
        {
            //print("\n2");
            if (Skip(newControls))
                return;
            c2.ControllerListener(newControls);
        }
    }
    public void ControllerListener3(ControlStruct newControls)
    {

        if (gameObject.activeInHierarchy && enabled)
        {
            //print("\n3");
            if (Skip(newControls))
                return;
            c3.ControllerListener(newControls);
        }
    }
    public void ControllerListener4(ControlStruct newControls)
    {

        if (gameObject.activeInHierarchy && enabled)
        {
            //print("\n4");
            if (Skip(newControls))
                return;
            c4.ControllerListener(newControls);
        }
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
