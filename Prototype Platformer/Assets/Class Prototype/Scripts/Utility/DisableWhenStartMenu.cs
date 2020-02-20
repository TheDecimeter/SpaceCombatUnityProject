using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenStartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(!FindObjectOfType<UndestroyableData>().isMenuOpened());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
