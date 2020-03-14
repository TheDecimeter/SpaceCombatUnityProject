using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlListener
{
    private ControlManager manager;
    private bool releaseR, releaseL, releaseB, releaseS;
    private ControlStruct _controllerStatus;

    public ControlListener(ControlManager manager)
    {
        this.manager = manager;
        _controllerStatus = new ControlStruct(ControlStruct.None);
    }

    public void ControllerListener(ControlStruct newControls)
    {

        if (!manager.gameObject.activeInHierarchy || !manager.enabled)
            return;

        _controllerStatus.combine(newControls);

        if (_controllerStatus.moveLeft < -.2)
        {
            if (releaseL)
            {
                releaseL = false;
                manager.controls.FireL();
            }
        }
        else if (_controllerStatus.moveLeft > -.1)
        {
            releaseL = true;
        }
        if (_controllerStatus.moveLeft > .2)
        {
            if (releaseR)
            {
                releaseR = false;
                manager.controls.FireR();
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
                manager.controls.FireS();
            }
        }
        else
            releaseS = true;
        if (_controllerStatus.B || _controllerStatus.action || _controllerStatus.inGameMenu)
        {
            if (releaseB)
            {
                releaseB = false;
                manager.controls.FireB();
            }
        }
        else
            releaseB = true;
    }
}
