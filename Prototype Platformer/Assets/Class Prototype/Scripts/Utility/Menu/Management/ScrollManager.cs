using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : ControlFirer
{
    public ControlEvents [] Controls;
    public ControlManager controlManager;
    public int atControl = 2;

    private delegate int next(int x);
    private int Ledge,Redge;

    private void Start()
    {
        Ledge = 0;
        Redge = Controls.Length-1;
        Controls[atControl].Home = true;
    }

    public void ScrollTo(ControlEvents controls)
    {

        if(Controls[atControl] == controls)
        {
            SwitchControl(atControl);
            return;
        }

        int r = distToElement(GetNext, controls);
        int l = distToElement(GetPrev, controls);
        if (l < r)
            for (int i = 0; i < l; ++i)
                ScrollLeft();
        else
            for (int i = 0; i < r; ++i)
                ScrollRight();

    }
    private int distToElement(next Next, ControlEvents target)
    {
        int r=0;
        int at = atControl;
        while(Controls[at]!=target)
        {
            r++;
            at = Next(at);
        }
        return r;
    }

    public void ScrollLeft()
    {
        Vector2 EdgeLoc= Controls[GetNext(Redge)].Loc();
        Vector2 loc = Controls[0].Loc();
        for (int i = 0; i < Controls.Length-1; ++i)
            Controls[i].MoveTo(Controls[GetNext(i)].Loc());
        Controls[Controls.Length - 1].MoveTo(loc);
        
        Controls[Redge].transform.position = EdgeLoc;

        Ledge = GetPrev(Ledge);
        Redge = GetPrev(Redge);
        SwitchControl(GetPrev(atControl));
    }
    public void ScrollRight()
    {
        Vector2 EdgeLoc = Controls[GetPrev(Ledge)].Loc();
        Vector2 loc = Controls[Controls.Length - 1].Loc();
        for (int i = Controls.Length - 1; i > 0; --i)
            Controls[i].MoveTo(Controls[GetPrev(i)].Loc());
        Controls[0].MoveTo(loc);

        Controls[Ledge].transform.position = EdgeLoc;

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
