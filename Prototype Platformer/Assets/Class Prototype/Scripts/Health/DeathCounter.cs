using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathCounter : MonoBehaviour
{
    public int DeathsToEvent = 2;
    public UnityEvent DeathEvent;
    private int deaths=0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playerDied(int playerID)
    {
        deaths++;
        if (deaths == DeathsToEvent)
            DeathEvent.Invoke();
    }
}
