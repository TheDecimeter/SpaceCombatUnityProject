using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalPlayerCountShowing : MonoBehaviour
{
    public int playerNum = 1;
    public bool showIfAllAI = true;
    // Start is called before the first frame update
    void Start()
    {
        int players = GetPlayers();
        //print("players shower" + players);
        if (players == 0 && showIfAllAI)
            return;
        if (playerNum > players)
            gameObject.SetActive(false);
    }

    private int GetPlayers()
    {
        int r = 0;
        FindObjectOfType<UndestroyableData>().GetPlayers((x) => { r = x; });
        return r;
    }
}
