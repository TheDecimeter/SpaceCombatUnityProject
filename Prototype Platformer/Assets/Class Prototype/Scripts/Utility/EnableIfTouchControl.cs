using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableIfTouchControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!GetTouch())
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool GetTouch()
    {
        bool r = false;
        FindObjectOfType<UndestroyableData>().GetTouchScreenControls((x) => { r = x; });
        return r;
    }
}
