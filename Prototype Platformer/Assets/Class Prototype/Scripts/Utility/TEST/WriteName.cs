using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WriteName : MonoBehaviour
{
    public Text text;
    public GameObject objectName;
    // Start is called before the first frame update
    void Start()
    {
        text.text = objectName.name;
    }

    // Update is called once per frame
    void Update()
    {
        if(objectName)
            text.text = objectName.name;
        else
            text.text = "Object not found";
    }
}
