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
        deaths = 0;
        Debug.LogWarning(" starting new match");
    }
    

    public void playerDied(int playerID)
    {
        Debug.LogWarning("Player died " + playerID);
        deaths++;
        if (deaths == DeathsToEvent)
        {
            Debug.LogWarning("Invoking launch pod");
            DeathEvent.Invoke();
        }
        if (deaths == 3)
        {
            foreach (Transform child in PlayerArray.transform)
                if(!child.gameObject.GetComponent<PlayerHealth>().isDead)
                    FindObjectOfType<UndestroyableData>().
                        IncreaseScore(child.gameObject.GetComponent<CharacterMovement_Physics>().
                        PlayerNumber, 1);

            Debug.LogWarning("Invoking one player remaining event");
            OnePlayerLeftEvent.Invoke();
        }
    }
}
