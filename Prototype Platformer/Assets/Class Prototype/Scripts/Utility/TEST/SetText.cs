using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{
    public string text;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
    
}
