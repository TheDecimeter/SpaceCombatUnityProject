using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathCounter : MonoBehaviour
{
    public GameObject PlayerArray;
    public int DeathsToEvent = 2;
    
    public UnityEvent DeathEvent;
    public UnityEvent OnePlayerLeftEvent;
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
        if (deaths == 3)
        {
            foreach (Transform child in PlayerArray.transform)
                if(!child.gameObject.GetComponent<PlayerHealth>().isDead)
                    FindObjectOfType<UndestroyableData>().
                        IncreaseScore(child.gameObject.GetComponent<CharacterMovement_Physics>().
                        PlayerNumber, 1);
            OnePlayerLeftEvent.Invoke();
        }
    }
}
