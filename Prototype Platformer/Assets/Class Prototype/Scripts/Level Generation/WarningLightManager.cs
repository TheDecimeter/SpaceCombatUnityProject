using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLightManager : MonoBehaviour
{
    [Header("The two types of lights")]
    public GameObject WarningLight;
    public GameObject DisabledRoomLight;

    [Header("Warning Light Intensity")]
    public float wMax = 30;
    public float wMin = 10;

    [Header("How much should Disabled Lights phase")]
    public int dMax = 100;
    public int dMin = 50;
    public int dExtremeMargin = 10;

    [Header("at what rate")]
    public int dFluctuation = 20;
    public int dFluctuationMargin = 10;
    
    public Vector3 podVect { get; set; }


    private List<DisabledLightNode> ActiveDisabledLights;


    private GameObject gActiveWarningLight;
    private Light ActiveWarningLight;
    private int warnFramesCount, frameCounter = 0;

    public Vector3 AsyncPosition { get; protected set; }
    public bool AsyncIncomming { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        AsyncIncomming = false;
        WarningLight.SetActive(false);
        DisabledRoomLight.SetActive(false);
        ActiveDisabledLights = new List<DisabledLightNode>();


        LevelRandomizer tmp = GetComponent<LevelRandomizer>();
        warnFramesCount = tmp.WarnForXManyFrames;
        if (warnFramesCount == 0) warnFramesCount++;
    }

    private void SetAsync(bool incomming)
    {
        if(incomming)
            AsyncPosition = WarningLight.transform.position;
        AsyncIncomming = incomming;
    }

    // Update is called once per frame
    void Update()
    {
        //adjust disabled room lights intensity here
        PhaseDisabledRoomLights();
    }
    void FixedUpdate()
    {
        //adjust warning light here
        ActiveWarningLight_AlterLight(frameCounter++, warnFramesCount);
    }
    
    public void ActiveWarningLight_AlterLight(int currentFrame, int endFrame)
    {
        if (gActiveWarningLight == null||!gActiveWarningLight.activeInHierarchy)
            return;
        if (currentFrame > endFrame)
        {
            gActiveWarningLight.SetActive(false);
            SetAsync(false);
            Vector3 t = gActiveWarningLight.transform.position;
            AddDisabledRoom(t.x, t.y, t.z);
            return;
        }
        ActiveWarningLight.intensity = wMin + ((wMax - wMin)* ((float)currentFrame / endFrame));
        //print("in warning light Intensity: " + ActiveWarningLight.intensity);
    }

    public void AddWarningLight(float x, float y, float z)
    {
        if (gActiveWarningLight == null)
            gActiveWarningLight = WarningLight;// Instantiate(WarningLight);
        gActiveWarningLight.SetActive(true);
        ActiveWarningLight = gActiveWarningLight.GetComponent<Light>();

        gActiveWarningLight.transform.position = new Vector3(x, y, z);
        SetAsync(true);
        ActiveWarningLight.intensity = wMin;
        //gActiveWarningLight.SetActive(true);
        frameCounter = 0;
    }

    public void AddDisabledRoom(float x, float y, float z)
    {
        //ActiveDisabledLights.Add(new DisabledLightNode(x, y, z,dMax,dMin,dExtremeMargin,dFluctuation,dFluctuationMargin, DisabledRoomLight));
    }

    public void reset()
    {
        foreach (DisabledLightNode n in ActiveDisabledLights)
            n.Destroy();
        ActiveDisabledLights = new List<DisabledLightNode>();

        if (gActiveWarningLight != null)
        {
            gActiveWarningLight.SetActive(false); //Destroy(gActiveWarningLight);
            SetAsync(false);
        }
    }

    public void PhaseDisabledRoomLights()
    {
        foreach (DisabledLightNode n in ActiveDisabledLights)
            n.AlterLight();
    }

    private class DisabledLightNode {
        public GameObject gLight;
        public Light light;
        public int maxIntensity, minIntensity;
        int currentIntensity, fluctuation;

        public DisabledLightNode(float x, float y, float z, int dMax, int dMin, int dExtremeMargin, int dFluctuation, int dFluctuationMargin,GameObject DisabledRoomLight)
        {
            maxIntensity = dMax + Random.Range(-dExtremeMargin, dExtremeMargin);
            minIntensity = dMin + Random.Range(-dExtremeMargin, dExtremeMargin);

            gLight= Instantiate(DisabledRoomLight);
            gLight.SetActive(true);

            gLight.transform.position = new Vector3(x, y, z);
            light = gLight.GetComponent<Light>();
            
            this.fluctuation = dFluctuation + Random.Range(-dFluctuationMargin, dFluctuationMargin);

            if (fluctuation > 0)
            {
                light.intensity = dMin;
                currentIntensity = dMin;
            }
            else
            {
                light.intensity = dMax;
                currentIntensity = dMax;
            }
        }
        public void AlterLight()
        {
            light.intensity += fluctuation;
            if (light.intensity > maxIntensity || light.intensity < minIntensity)
                fluctuation *= -1;
        }

        public void Destroy()
        {
            MonoBehaviour.Destroy(gLight);
        }

    }

}
