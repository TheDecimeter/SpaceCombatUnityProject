using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    public SkinnedMeshRenderer mat;
    private Color oldColor, damageColor;
    private bool damageShown;
    private const string emission = "_EmissionColor";
    [Header("How many flickers happen per damage")]
    private int numFlickers = 3;
    private int curntFlicker=0;
    [Header("How long should each flicker last")]
    private float damageSeconds = .005f;
    private float currentDamage = 0;
    // Start is called before the first frame update
    void Start()
    {
        damageShown = false;
        damageColor = new Color(1, 1, 1);
        oldColor = mat.material.GetColor(emission);
        //mat.material.SetColor(emission, damageColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentDamage >= damageSeconds)
        {
            if (curntFlicker == 0)
                return;
            else
            {
                currentDamage = 0;
                if (damageShown)
                {
                    mat.material.SetColor(emission, oldColor);
                    damageShown = false;
                    curntFlicker--;
                }
                else
                {
                    mat.material.SetColor(emission, damageColor);
                    damageShown = true;
                }

            }
        }
        else
            currentDamage += Time.deltaTime;
    }
    void LateUpdate()
    {
    }

    public void animateDamage()
    {
        if (curntFlicker>0)
            return;
        mat.material.SetColor(emission, damageColor);
        damageShown = true;
        curntFlicker = numFlickers;
        currentDamage = 0;
    }
}
