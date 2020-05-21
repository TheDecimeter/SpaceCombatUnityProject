using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemSpecificDisplay : MonoBehaviour
{
    enum OS { Windows, Linux, Mac, Android};

    [System.Serializable]
    struct Display
    {
        public OS Platform;
        public GameObject Window;
    }

    [SerializeField]
    private Display [] Displays;

    [SerializeField]
    private GameObject DefaultDisplay;

    void Start()
    {
        bool done = false;
        Display d;
#if UNITY_STANDALONE_WIN
        if (TryGet(OS.Windows, out d))
            done = true;
#elif UNITY_STANDALONE_LINUX
        if (TryGet(OS.Linux, out d))
            done = true;
#elif UNITY_STANDALONE_OSX
        if (TryGet(OS.Mac, out d))
            done = true;
#elif UNITY_ANDROID
        if (TryGet(OS.Android, out d))
            done = true;
#endif
        if (done)
        {
            DefaultDisplay.SetActive(false);
            d.Window.SetActive(true);
        }
    }

    private bool TryGet(OS os, out Display display)
    {
        foreach(Display d in Displays)
        {
            if(d.Platform==os)
            {
                display = d;
                return true;
            }
        }
        display = new Display();
        return false;
    }
}
