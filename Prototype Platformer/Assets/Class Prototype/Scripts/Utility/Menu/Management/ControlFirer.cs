using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ControlFirer : MonoBehaviour
{
    private bool _home;
    public ControlFirer MenuChild;
    public GameObject HideIfNoFocus;

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
                if (HideIfNoFocus)
                    HideIfNoFocus.SetActive(false);
                if (MenuChild)
                {
                    MenuChild.Home = value;
                    MenuChild.FireB();
                }
            }
            else
            {
                if (MenuChild)
                    MenuChild.Home = value;
            }
        }
    }

    partial void FireL();
    partial void FireR();
    partial void FireS();
    partial void FireB();
}
