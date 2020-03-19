using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRectSetter : MonoBehaviour
{
    public int playerNum = 1;
    public bool vertical=true;
    private int playerAmmount;
    // Start is called before the first frame update
    void Start()
    {
        playerAmmount = GetPlayers();
        SetShared();
    }
    
    public void SetFull()
    {
        Camera cam = GetComponent<Camera>();
        if (!cam.enabled)
            return;
        cam.rect = new Rect(0, 0, 1, 1);
    }

    public void SetShared()
    {
        switch (playerAmmount)
        {
            case 1:
                set1();
                break;
            case 2:
                set2(vertical);
                break;
            case 3:
                set4();
                break;
            default:
                set4();
                break;
        }
    }

    private void set4()
    {
        float x = 0, y = 0;
        if (playerNum < 3)
            y = .5f;
        if ((playerNum & 1) == 0)
            x = .5f;

        Camera cam = GetComponent<Camera>();
        cam.rect = new Rect(x, y, .5f, .5f);
    }
    private void set2(bool vertical)
    {
        Camera cam = GetComponent<Camera>();

        if (vertical)
        {
            if(playerNum==2)
            {
                cam = GetComponent<Camera>();
                cam.rect = new Rect(0, 0, 1f, .5f);
                return;
            }
            if (playerNum == 1)
            {
                cam = GetComponent<Camera>();
                cam.rect = new Rect(0, .5f, 1f, .5f);
                return;
            }
            cam.enabled = false;
            return;
        }

        float x = 0;
        if (playerNum == 2)
            x = .5f;
        if (playerNum > 2)
            cam.enabled = false;
        cam.rect = new Rect(x, 0, .5f, 1f);
    }
    private void set1()
    {
        Camera cam = GetComponent<Camera>();
        if (playerNum > 1)
            cam.enabled = false;
        cam.rect = new Rect(0, 0, 1f, 1f);
    }


    private int GetPlayers()
    {
        int r = 0;
        FindObjectOfType<UndestroyableData>().GetPlayers((x) => { r = x; });
        return r;
    }
}
