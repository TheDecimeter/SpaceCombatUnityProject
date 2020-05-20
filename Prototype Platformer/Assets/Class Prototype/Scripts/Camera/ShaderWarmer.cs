using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderWarmer : MonoBehaviour
{
    private static bool doIt=true;
    // Start is called before the first frame update
    void Start()
    {
        if(doIt)
        {
            doIt = false;
            //Shader.WarmupAllShaders();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
