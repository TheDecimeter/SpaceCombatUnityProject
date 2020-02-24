using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    float startx, starty, startz, xDim, yDim;
    int warnFrames, asteroidTimer = 0, warnTimer = 0, nextThrow;
    bool hasWarned;
    GameObject[] _asteroidArray;
    List<GameObject> chuckedAsteroids;
     float xOffset;
     float yOffset;

    [Header("Where are asteroid templates stored:")]
    public GameObject asteroidArray;
    public int AsteroidSpeed=70;
    [Header("impact margin from center")]
    public float xMargin = 5;
    public float yMargin = 3;
    [Header("Frame count to chuck asteroid")]
    public int chuckEvery=20;
    public int plusOrMinus = 5;

    private WarningLightManager daLight;

    void Start()
    {
        LevelRandomizer tmp = GetComponent<LevelRandomizer>();
        xOffset = tmp.xOffset;
        yOffset = tmp.yOffset;
        //print("offsets " + xOffset + " " + yOffset);
        daLight = GetComponent<WarningLightManager>();

        Init(tmp.startX, tmp.startY, tmp.startZ, tmp.xTileSize, tmp.yTileSize, tmp.WarnForXManyFrames);
    }

    public void Init(float startx, float starty, float startz, float xDim, float yDim, int warnFrames)
    {
        this.startx = startx;
        this.starty = starty;
        this.startz = startz;
        this.xDim = xDim;
        this.yDim = yDim;
        this.warnFrames = warnFrames;
        hasWarned = false;

        //populate the randomly accessable asteroid array
        int count = 0;
        foreach (Transform child in asteroidArray.transform)
            count++;
        _asteroidArray = new GameObject[count];

        count = 0;
        foreach (Transform child in asteroidArray.transform)
        {
            _asteroidArray[count] = child.gameObject;
            _asteroidArray[count++].SetActive(false);
        }

        chuckedAsteroids = new List<GameObject>();
    }

    public bool warn(bool doIwarn, int whereX, int whereY)
    {
        if (doIwarn)
        {
            float xx = startx + whereX * xDim + xOffset, yy = starty + whereY * yDim + yOffset;
            if (!hasWarned)
            {
                daLight.AddWarningLight(xx, yy, startz - 1);
                hasWarned = true;
                asteroidTimer = 0;
                warnTimer = 0;
                nextThrow = chuckEvery + Random.Range(-plusOrMinus, plusOrMinus);
            }
            chuckAsteroids(xx, yy);
        }
        else if (hasWarned)
        {
            hasWarned = false;
            //foreach (GameObject g in chuckedAsteroids)
            //    Destroy(g);
            chuckedAsteroids = new List<GameObject>();
            return true;
        }
        return false;
    }
    private void chuckAsteroids(float whereX, float whereY)
    {
        if (asteroidTimer == 0)
        {
            if(warnTimer<warnFrames/2)
                initiateAsteroidChuckin(whereX, whereY);
        }
        else
        {
            if (asteroidTimer == nextThrow)
            {
                asteroidTimer = -1;
                nextThrow = chuckEvery + Random.Range(-plusOrMinus, plusOrMinus);
            }
        }
        asteroidTimer++;
        warnTimer++;

    }
    private void initiateAsteroidChuckin(float whereX, float whereY)
    {
        GameObject tmp = Instantiate(_asteroidArray[Random.Range(0,_asteroidArray.Length)]);
        tmp.SetActive(true);
        Vector3 target = new Vector3(whereX+Random.Range(-xMargin,xMargin), whereY + Random.Range(-yMargin, yMargin), startz);
        tmp.transform.position = new Vector3(whereX, whereY, startz - 25);

        Rigidbody rb = tmp.GetComponent<Rigidbody>();
        rb.AddForce((target - tmp.transform.position) * AsteroidSpeed, ForceMode.Acceleration);
        rb.AddTorque(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100), ForceMode.VelocityChange);

        chuckedAsteroids.Add(tmp);
    }
}
