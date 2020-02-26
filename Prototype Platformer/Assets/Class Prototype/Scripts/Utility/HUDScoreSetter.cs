using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScoreSetter : MonoBehaviour
{
    private static readonly string[] nameOf = { "Bibbs", "Leslie", "Giggles", "Hobbs" };

    public GameObject HUD;
    // Start is called before the first frame update
    void Start()
    {
        int playerNum = 0;
        foreach (int i in FindObjectOfType<UndestroyableData>().GetScore())
            if(i>0)
                HUD.transform.Find("HUD_" + nameOf[playerNum++] + "/HUD/Score").gameObject.GetComponent<Text>().text=""+i;
            else
                HUD.transform.Find("HUD_"+nameOf[playerNum++] + "/HUD/Score").gameObject.GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
