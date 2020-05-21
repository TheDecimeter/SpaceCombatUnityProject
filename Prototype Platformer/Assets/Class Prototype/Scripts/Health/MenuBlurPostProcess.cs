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
        
        HardBlendMat.SetFloat("_Intensity", .08f);
        cache = null;
    }

    //void FixedUpdate()
    //{
    //    Rend(Source, Dest);
    //}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        if (cache == null)
        {
            RenderTexture temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
            cache = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.RGB111110Float);
            cache.Create();
            Graphics.Blit(source, temporaryTexture, BlurMat, 0);
            Graphics.Blit(temporaryTexture, cache, BlurMat, 1);
            HardBlendMat.SetTexture("_BTex", cache);
            cache.MarkRestoreExpected();
            RenderTexture.ReleaseTemporary(temporaryTexture);
        }

        RenderTexture temporaryTexture2 = RenderTexture.GetTemporary(source.width, source.height);

        Graphics.Blit(source, temporaryTexture2, HardBlendMat);
        Graphics.Blit(temporaryTexture2, cache);
        Graphics.Blit(temporaryTexture2, destination);

        RenderTexture.ReleaseTemporary(temporaryTexture2);
    }

    private void OnDisable()
    {
        cache = null;
    }
    
    
    
}
