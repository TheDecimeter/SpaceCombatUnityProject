using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{

    private int delay = 0;
    public TextMesh info;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (delay == 0)
            info.text = "";
        else delay--;
    }

    public void say(string text, int delay)
    {
        this.delay = delay;
        info.text = text;
    }
}
