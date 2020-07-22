﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : ControlFirer
{
    public ControlEvents[] Controls;
    public ControlEvents[] WindowsControls;
    public ControlManager controlManager;
    public int atControl = 2, WindowsAtControl=2;

    private delegate int next(int x);
    private int Ledge,Redge;
    private GameObject Lloc, Rloc;

    private void Start()
    {
        SetSystemControls();
        Ledge = 0;
        Redge = Controls.Length-1;
        Controls[atControl].Home = true;
        SetEndLocs();

    }

    private void SetSystemControls()
    {
        #if UNITY_STANDALONE_WIN
            MakeControlsActive(WindowsControls, WindowsAtControl);
        #endif
    }

    private void MakeControlsActive(ControlEvents[] controls, int atControl)
    {
        if (controls == null || controls.Length == 0)
            return;

        foreach (ControlEvents c in Controls)
            c.gameObject.SetActive(false);
        foreach (ControlEvents c in controls)
            c.gameObject.SetActive(true);
        Controls = controls;
        this.atControl = atControl;
    }

    /// <summary>
    /// Set Lloc and Rloc to create off screen staging points for the
    /// menu objects which "wrapped" around the screen
    /// </summary>
    private void SetEndLocs()
    {
        GameObject l = new GameObject("Left Edge loc for " + name);
        GameObject r = new GameObject("Right Edge loc for " + name);
        if (Controls.Length < 2)
        {
            Vector2 home = transform.localPosition;
            if (Controls.Length > 0)
                home = Controls[0].transform.localPosition;

            l.transform.SetParent(transform.parent);
            r.transform.SetParent(transform.parent);
            l.transform.localPosition = home + new Vector2(-500, 0);
            r.transform.localPosition = home + new Vector2(500, 0);
            Lloc = l;
            Rloc = r;
            return;
        }

        l.transform.SetParent(Controls[0].transform.parent);
        r.transform.SetParent(Controls[Controls.Length - 1].transform.parent);
        Vector2 Rdir = Controls[Controls.Length - 1].transform.localPosition - Controls[Controls.Length - 2].transform.localPosition;
        Vector2 Ldir = Controls[0].transform.localPosition - Controls[1].transform.localPosition;
        l.transform.localPosition = (Vector2)Controls[0].transform.localPosition + Ldir;
        r.transform.localPosition = (Vector2)Controls[Controls.Length - 1].transform.localPosition + Rdir;
        Lloc = l;
        Rloc = r;


    }

    public void ScrollTo(ControlEvents controls)
    {
        
        if(Controls[atControl] == controls)
        {
            SwitchControl(atControl);
            return;
        }

        if (Controls.Length == 1)
            return;
        int r = distToElement(GetNext, controls);
        int l = distToElement(GetPrev, controls);

        print("scrollTO l:" + l + " r:" + r);

        if (l < r)
        {
            for (int i = 0; i < l; ++i)
                ScrollLeft();
        }
        else
        {
            for (int i = 0; i < r; ++i)
                ScrollRight();
        }

    }
    
    public void ScrollDirectlyTo(ControlEvents controls)
    {
        if (Controls.Length == 1)
            return;
        int r = distToElement(GetNext, controls);
        for (int j = 0; j < r; ++j)
        {
            Vector2 loc = Controls[Controls.Length - 1].Loc();
            for (int i = Controls.Length - 1; i > 0; --i)
                Controls[i].MoveDirectlyTo(Controls[GetPrev(i)].Loc());
            Controls[0].MoveDirectlyTo(loc);

            Ledge = GetNext(Ledge);
            Redge = GetNext(Redge);
            SwitchControl(GetNext(atControl));
        }
    }

    private int distToElement(next Next, ControlEvents target)
    {
        int r=0;
        int at = atControl;
        while(Controls[at]!=target)
        {
            r++;
            at = Next(at);
            if (r > Controls.Length)
                throw new KeyNotFoundException(gameObject.name+" doesn't contain " + target.name);
        }
        return r;
    }

    public void ScrollLeft()
    {
        if (Controls.Length == 1)
            return;
        //Vector2 EdgeLoc= Controls[GetNext(Redge)].Loc();
        Vector2 loc = Controls[0].Loc();
        for (int i = 0; i < Controls.Length-1; ++i)
            Controls[i].MoveTo(Controls[GetNext(i)].Loc());
        Controls[Controls.Length - 1].MoveTo(loc);
        
        Controls[Redge].transform.position = Lloc.transform.position;

        Ledge = GetPrev(Ledge);
        Redge = GetPrev(Redge);
        SwitchControl(GetPrev(atControl));
    }
    public void ScrollRight()
    {
        if (Controls.Length == 1)
            return;
        //Vector2 EdgeLoc = Controls[GetPrev(Ledge)].Loc();
        Vector2 loc = Controls[Controls.Length - 1].Loc();
        for (int i = Controls.Length - 1; i > 0; --i)
            Controls[i].MoveTo(Controls[GetPrev(i)].Loc());
        Controls[0].MoveTo(loc);

        Controls[Ledge].transform.position = Rloc.transform.position;

        Ledge = GetNext(Ledge);
        Redge = GetNext(Redge);
        SwitchControl(GetNext(atControl));
    }
    

    private void SwitchControl(int to)
    {

        Controls[atControl].Home = false;
        atControl = to;
        controlManager.controls = Controls[atControl];
        Controls[atControl].Home = true;
    }

    private int GetNext(int i)
    {
        if (i == Controls.Length-1)
            return 0;
        return i + 1;
    }
    private int GetPrev(int i)
    {
        if (i == 0)
            return Controls.Length - 1;
        return i - 1;
    }

    public override void FireL()
    {
        Controls[atControl].FireL();
    }

    public override void FireR()
    {
        Controls[atControl].FireR();
    }

    public override void FireS()
    {
        Controls[atControl].FireS();
    }

    public override void FireB()
    {
        Controls[atControl].FireB();
    }

    public override ControlFirer Child()
    {
        return Controls[atControl].Child();
    }
    public override bool HasChild(ControlFirer c)
    {
        if (this == c)
            return true;
        foreach (ControlFirer f in Controls)
            if(f.HasChild(c))
                return true;

        return false;
    }


    public override void SetHide(bool show)
    {
        //gameObject.SetActive(show);
        Controls[atControl].SetHide(show);
    }
}
