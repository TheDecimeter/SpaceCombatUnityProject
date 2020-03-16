using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalPlayerCountShowing : MonoBehaviour
{
    public int playerNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        int players = GetPlayers();
        //print("players shower" + players);
        if (players == 0)
            return;
        if (playerNum > GetPlayers())
            gameObject.SetActive(false);
    }

    private int GetPlayers()
    {
        int r = 0;
        FindObjectOfType<UndestroyableData>().GetPlayers((x) => { r = x; });
        return r;
    }
}
