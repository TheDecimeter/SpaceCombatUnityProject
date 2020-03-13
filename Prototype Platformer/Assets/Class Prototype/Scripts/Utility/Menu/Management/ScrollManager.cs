﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public ControlEvents [] Controls;
    public ControlManager controlManager;
    public int atControl = 2;
    // Start is called before the first frame update
    void Start()
    {
        if (atControl > Controls.Length-1)
            atControl = 0;
        controlManager.controls = Controls[atControl];
    }


    public void ScrollLeft()
    {
        Vector2 loc = Controls[0].Loc();
        for (int i = 0; i < Controls.Length-1; ++i)
            Controls[i].MoveTo(Controls[GetNext(i)].Loc());
        Controls[Controls.Length - 1].MoveTo(loc);

        atControl = GetNext(atControl);
        controlManager.controls = Controls[atControl];
    }
    public void ScrollRight()
    {
        Vector2 loc = Controls[Controls.Length - 1].Loc();
        for (int i = Controls.Length - 1; i > 0; --i)
            Controls[i].MoveTo(Controls[GetPrev(i)].Loc());
        Controls[0].MoveTo(loc);
        
        atControl = GetPrev(atControl);
        controlManager.controls = Controls[atControl];
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
}
