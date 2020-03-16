using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetValues : MonoBehaviour
{
    // Start is called before the first frame update
    

    private int GetPlayers(UndestroyableData data)
    {
        int r = 0;
        data.GetPlayers((x) => { r = x; });
        return r;
    }
    private int GetRounds(UndestroyableData data)
    {
        int r = 0;
        data.GetRoundsE((x) => { r = x; });
        return r;
    }

    public void Set()
    {
        UndestroyableData data = FindObjectOfType<UndestroyableData>();
        data.SetRounds(GetRounds(data));
    }
}
