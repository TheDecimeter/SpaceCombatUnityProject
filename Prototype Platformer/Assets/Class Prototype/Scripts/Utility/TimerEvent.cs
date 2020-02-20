using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerEvent : MonoBehaviour
{
    public float DelaySeconds = 1;
    private float currentSeconds = 0;
    public UnityEvent onTimerComplete;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSeconds >= DelaySeconds)
        {
            onTimerComplete.Invoke();
        }
        else
            currentSeconds += Time.deltaTime;
    }

    public void restart()
    {
        currentSeconds = 0;
    }
    public void reset(float DelaySeconds)
    {
        this.DelaySeconds = DelaySeconds;
        restart();
    }
}
