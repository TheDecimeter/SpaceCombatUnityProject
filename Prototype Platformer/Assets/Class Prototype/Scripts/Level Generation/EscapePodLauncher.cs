using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodLauncher : MonoBehaviour
{
    public GameObject EscapePod;
    public int LaunchSpeed=30;
    
    private GameObject WarningLight;
    
    private float wMax = 30;
    private float wMin = 10;

    private GameObject gActiveWarningLight;
    private Light ActiveWarningLight;
    private int warnFramesCount, frameCounter = 0;

    void Start()
    {
        LevelRandomizer tmp = GetComponent<LevelRandomizer>();
        warnFramesCount = tmp.WarnForXManyFrames;
        if (warnFramesCount == 0) warnFramesCount++;

        WarningLightManager tmp2 = GetComponent<WarningLightManager>();
        WarningLight = tmp2.WarningLight;
        wMax = tmp2.wMax;
        wMin = tmp2.wMin;
    }

    void FixedUpdate()
    {
        //adjust warning light here
        ActiveWarningLight_AlterLight(frameCounter++, warnFramesCount);
    }

    public void ActiveWarningLight_AlterLight(int currentFrame, int endFrame)
    {
        if (gActiveWarningLight == null || !gActiveWarningLight.activeInHierarchy)
            return;
        if (currentFrame > endFrame)
        {
            gActiveWarningLight.SetActive(false);
            return;
        }
        ActiveWarningLight.intensity = wMin + ((wMax - wMin) * ((float)currentFrame / endFrame));
        //print("in warning light Intensity: " + ActiveWarningLight.intensity);
    }

    public void AddWarningLight(float x, float y, float z)
    {
        if (gActiveWarningLight == null)
            gActiveWarningLight = Instantiate(WarningLight);

        ActiveWarningLight = gActiveWarningLight.GetComponent<Light>();

        gActiveWarningLight.transform.position = new Vector3(x, y, z);
        ActiveWarningLight.intensity = wMin;
        gActiveWarningLight.SetActive(true);
        frameCounter = 0;
    }


    public void Launch(float x, float y, float z)
    {
        AddWarningLight(x, y, z-1);

        GameObject tmp = Instantiate(EscapePod);
        tmp.SetActive(true);
        Vector3 target = new Vector3(x, y, z);
        tmp.transform.position = new Vector3(x, y, z - 25);
        tmp.GetComponent<Rigidbody>().AddForce((target - tmp.transform.position) * LaunchSpeed, ForceMode.Acceleration);
    }
}
