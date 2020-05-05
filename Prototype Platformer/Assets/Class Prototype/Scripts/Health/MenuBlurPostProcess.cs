using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBlurPostProcess : MonoBehaviour
{
    public Material HardBlendMat;
    public Material BlurMat;

    private RenderTexture cache;

    void Start()
    {
        HardBlendMat = Instantiate(HardBlendMat);
        
        HardBlendMat.SetFloat("_Intensity", .1f);
        
        cache = null;
    }

    //void FixedUpdate()
    //{
    //    Rend(Source, Dest);
    //}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        if (cache == null)
        {
            cache = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.RGB111110Float);
            cache.Create();
            Graphics.Blit(source, temporaryTexture, BlurMat, 0);
            Graphics.Blit(temporaryTexture, cache, BlurMat, 1);
            HardBlendMat.SetTexture("_BTex", cache);
        }

        Graphics.Blit(source, temporaryTexture, HardBlendMat);
        Graphics.Blit(temporaryTexture, cache);
        Graphics.Blit(temporaryTexture, destination);
        RenderTexture.ReleaseTemporary(temporaryTexture);
    }

    private void OnDisable()
    {
        cache = null;
    }
    
    
    
}
