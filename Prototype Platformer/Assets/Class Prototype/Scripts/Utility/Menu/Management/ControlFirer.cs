using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlFirer : MonoBehaviour
{
    private bool _home;
    public bool Active { get; set; }

    public bool Home
    {
        get
        {
            return _home;
        }
        set
        {
            if (_home == value)
                return;
            _home = value;

            if (!value)
            {
                SetHide(false);
                if (Child())
                {
                    Child().Home = value;
                    Child().FireB();
                }
            }
            else
            {
                if (Child())
                    Child().Home = value;
            }
        }
    }

    public abstract void FireL();
    public abstract void FireR();
    public abstract void FireS();
    public abstract void FireB();
    public abstract ControlFirer Child();
    public abstract bool HasChild(ControlFirer c);
    public abstract void SetHide(bool hide);
}
